using System;
using System.Collections.Generic;
using System.Linq;
using Billiard3D.VectorMath;
using System.Runtime.CompilerServices;
using static System.Math;

namespace Billiard3D.Track
{
    internal class Sphere : ITrackBoundary, IEquatable<Sphere>
    {
        public Vector3D Center { get; }
        private double Radius { get; }

        public List<Vector3D> HitPoints { get; } = new List<Vector3D>(10);

        public string BoundaryName { get; set; }

        public IPointChecker Checker { get; set; }
        private const double Confidence = 0.00005;

        public Sphere(Vector3D center, double radius)
        {
            Center = center;
            Radius = radius;
        }

        public Sphere(Vector3D center, double radius, IPointChecker checker)
        {
            Center = center;
            Radius = radius;
            Checker = checker;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(Sphere other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(Center, other.Center) && Radius.Equals(other.Radius);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IEnumerable<Vector3D> GetIntersectionPoints(in Line line)
        {
            var linePoint = line.BasePoint;
            var lineDir = line.Direction;
            var discriminant = Pow(lineDir * (line.BasePoint - Center), 2)
                               - Pow(Vector3D.AbsoluteValue(line.BasePoint - Center), 2) + Pow(Radius, 2);
            if (discriminant < 0)
                return Enumerable.Empty<Vector3D>();
            var plus = -(lineDir * (line.BasePoint - Center)) + Sqrt(discriminant);
            var minus = -(lineDir * (line.BasePoint - Center)) - Sqrt(discriminant);
            if (discriminant < Confidence)
            {
                var result = plus > 0
                    ? new List<Vector3D> {line.BasePoint + (plus * line.Direction) }.Where(Checker.IsPointOnTheCorrectSide)
                    : Enumerable.Empty<Vector3D>();
                return result;
            }

            var equationResults = new List<double> {plus, minus}.Where(x => x > Confidence);
            var results = equationResults.Select(x => linePoint + (x * lineDir));
            return results.Where(Checker.IsPointOnTheCorrectSide);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Line LineAfterHit(in Line incoming, in Vector3D hitPoint)
        {
            HitPoints.Add(hitPoint);
            var line = new Line(Center, hitPoint);
            var normalVector = -1 * line.Direction.Normalize();

            var newDirection = (2 * (-1 * incoming.Direction.Normalize() * normalVector) * normalVector)
                               + incoming.Direction.Normalize();
            return Line.FromPointAndDirection(hitPoint, newDirection);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsInCorrectPosition(Line ball) => Checker.IsPointOnTheCorrectSide(ball.BasePoint);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override bool Equals(object obj)
        {
            if (obj is null) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == GetType() && Equals((Sphere) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return Center.GetHashCode() * 397 ^ Radius.GetHashCode();
            }
        }
    }
}