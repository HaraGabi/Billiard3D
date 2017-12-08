using System;
using System.Collections.Generic;
using System.Linq;
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

        public (IEnumerable<(Vector3D, double)>, ITrackObject) GetIntersectionPoints(Line line)
        {
            var lineDir = line.Direction.Normalize();
            var discriminant = Pow(lineDir * (line.PointA - Center), 2) -
                               Pow(Vector3D.AbsoluteValue(line.PointA - Center), 2) + Pow(Radius, 2);
            if (discriminant < 0)
            {
                // no intersection
                return (Enumerable.Empty<(Vector3D, double)>(), this);
            }
            var plus = -(lineDir * (line.PointA - Center)) + Sqrt(discriminant);
            var minus = -(lineDir * (line.PointA - Center)) - Sqrt(discriminant);
            if (discriminant < 0.000005)
            {
                var result = plus > 0 ? new List<(Vector3D, double)> { (line.PointA + plus * line.Direction, plus) } : Enumerable.Empty<(Vector3D, double)>();
                return (result, this);
            }
            var results = new List<(Vector3D, double)>
            {
                (line.PointA + plus * line.Direction, plus),
                (line.PointA + minus * line.Direction, minus)
            }.Where(x => x.Item2 > 0);
            return (results, this);
        }
        //
        public Line LineAfterHit(Line incoming, Vector3D hitPoint)
        {
            HittedPoints.Add(hitPoint);
            var line = new Line(Center, hitPoint);
            var normalVector = -1 * line.Direction.Normalize();

            var newDirection = 2 * (-1 * incoming.Direction.Normalize() * normalVector) * normalVector +
                               incoming.Direction.Normalize();
            return Line.FromPointAndDirection(hitPoint, newDirection);
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