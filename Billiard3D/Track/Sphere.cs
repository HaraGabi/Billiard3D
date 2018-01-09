using System;
using System.Collections.Generic;
using System.Linq;
using Billiard3D.VectorMath;
using static System.Math;

namespace Billiard3D.Track
{
    internal class Sphere : ITrackObject, IEquatable<Sphere>
    {
        private const double Confidence = 0.00005;
        public Vector3D Center { get; }
        private double Radius { get; }

        public Sphere(Vector3D center, double radius)
        {
            Center = center;
            Radius = radius;
        }

        public bool Equals(Sphere other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(Center, other.Center) && Radius.Equals(other.Radius);
        }

        public List<Vector3D> HitPoints { get; } = new List<Vector3D>(10);

        public IEnumerable<Vector3D> GetIntersectionPoints(Line line)
        {
            var linePoint = line.PointA;
            var lineDir = line.Direction;
            var discriminant = Pow(lineDir * (line.PointA - Center), 2) -
                               Pow(Vector3D.AbsoluteValue(line.PointA - Center), 2) + Pow(Radius, 2);
            if (discriminant < 0)
                return Enumerable.Empty<Vector3D>();
            var plus = -(lineDir * (line.PointA - Center)) + Sqrt(discriminant);
            var minus = -(lineDir * (line.PointA - Center)) - Sqrt(discriminant);
            if (discriminant < Confidence)
            {
                var result = plus > 0
                    ? new List<Vector3D> {line.PointA + plus * line.Direction}.Where(Checker.IsPointOnTheCorrectSide)
                    : Enumerable.Empty<Vector3D>();
                return result;
            }

            var equationResults = new List<double> {plus, minus}.Where(x => x > Confidence);
            var results = equationResults.Select(x => linePoint + x * lineDir);
            return results.Where(Checker.IsPointOnTheCorrectSide);
        }

        //
        public Line LineAfterHit(in Line incoming, in Vector3D hitPoint)
        {
            HitPoints.Add(hitPoint);
            var line = new Line(Center, hitPoint);
            var normalVector = -1 * line.Direction.Normalize();

            var newDirection = 2 * (-1 * incoming.Direction.Normalize() * normalVector) * normalVector +
                               incoming.Direction.Normalize();
            return Line.FromPointAndDirection(hitPoint, newDirection);
        }

        public bool IsInCorrectPosition(Line ball)
        {
            return Checker.IsPointOnTheCorrectSide(ball.PointA);
        }

        public string ObjectName { get; set; }

        public PointChecker Checker { get; set; }

        public override bool Equals(object obj)
        {
            if (obj is null) return false;
            if (ReferenceEquals(this, obj)) return true;
            return (obj.GetType() == GetType()) && Equals((Sphere) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ( Center.GetHashCode() * 397) ^ Radius.GetHashCode();
            }
        }
    }
}