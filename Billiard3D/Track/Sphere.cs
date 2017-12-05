using System;
using System.Collections.Generic;
using Billiard3D.VectorMath;
using static System.Math;

namespace Billiard3D.Track
{
    internal class Sphere : ITrackObject, IEquatable<Sphere>
    {
        public Sphere(Vector3D center, double radius)
        {
            Center = center;
            Radius = radius;
        }

        public Vector3D Center { get; }
        public double Radius { get; }

        public List<Vector3D> HittedPoints { get; } = new List<Vector3D>();

        public bool Equals(Sphere other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(Center, other.Center) && Radius.Equals(other.Radius);
        }

        public IEnumerable<Vector3D> GetIntersectionPoints(Line line)
        {
            var lineDir = line.Direction.Normalize();
            var discriminant = Pow(lineDir * (line.PointA - Center), 2) -
                               Pow(Vector3D.AbsoluteValue(line.PointA - Center), 2) + Pow(Radius, 2);
            if (discriminant < 0)
            {
                // no intersection
                yield break;
            }
            if (discriminant < 0.000005)
            {
                yield return line.PointA + (-(lineDir * (line.PointA - Center)) + Sqrt(discriminant)) * line.Direction;
                yield break;
            }
            yield return line.PointA + (-(lineDir * (line.PointA - Center)) + Sqrt(discriminant)) * line.Direction;
            yield return line.PointA + (-(lineDir * (line.PointA - Center)) - Sqrt(discriminant)) * line.Direction;
        }

        public Line LineAfterHit(Line incoming, Vector3D hittedPoint)
        {
            HittedPoints.Add(hittedPoint);
            var line = new Line(Center, hittedPoint);
            var normalVector = line.Direction.Normalize();

            var newDirection = 2 * (-1 * incoming.Direction.Normalize() * normalVector) * normalVector +
                               incoming.Direction.Normalize();
            return Line.FromPointAndDirection(hittedPoint, newDirection);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return (obj.GetType() == GetType()) && Equals((Sphere) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Center != null ? Center.GetHashCode() : 0) * 397) ^ Radius.GetHashCode();
            }
        }
    }
}