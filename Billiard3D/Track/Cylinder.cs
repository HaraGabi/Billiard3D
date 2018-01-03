using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Billiard3D.VectorMath;

namespace Billiard3D.Track
{
    internal class Cylinder : ITrackObject, IEquatable<Cylinder>, IEnumerable<Vector3D>
    {
        private const double Confidence = 0.00005;
        private readonly double _cylinderHeight;
        private double Radius { get; }

        public Vector3D TopCenter { get; }

        public Vector3D BottomCenter { get; }

        private PointChecker Checker { get; }

        public Cylinder(Vector3D top, Vector3D bottom, PointChecker checker, double radius)
        {
            Checker = checker;
            Radius = radius;
            TopCenter = top;
            BottomCenter = bottom;
            _cylinderHeight = Vector3D.AbsoluteValue(BottomCenter - TopCenter);
        }

        public IEnumerator<Vector3D> GetEnumerator()
        {
            yield return TopCenter;
            yield return BottomCenter;
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public bool Equals(Cylinder other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            return Radius.Equals(other.Radius) && this.Contains(other.TopCenter) && this.Contains(other.BottomCenter);
        }

        public List<Vector3D> HitPoints { get; } = new List<Vector3D>(1000);

        /// <summary>
        ///     Gets the necessary coefficient for the line to reach cylinder
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        public IEnumerable<Vector3D> GetIntersectionPoints(Line line)
        {
            var baseLine = new Line(TopCenter, BottomCenter);

            var v = line.Direction.Normalize();
            var va = baseLine.Direction.Normalize();
            var deltaP = line.PointA - TopCenter;

            var kisA = v - v * va * va;
            var kisC = deltaP - deltaP * va * va;

            var a = kisA * kisA;
            var b = 2 * (kisA * kisC);
            var c = kisC * kisC - Math.Pow(Radius, 2);

            var positive = (-b + Math.Sqrt(Math.Pow(b, 2) - 4 * a * c)) / (2 * a);
            var negative = (-b - Math.Sqrt(Math.Pow(b, 2) - 4 * a * c)) / (2 * a);

            var result = new List<Vector3D>
            {
                line.PointA + positive * line.Direction,
                line.PointA + negative * line.Direction
            }.Where(x => InsideTheCylinder(x) && Checker.IsPointOnTheCorrectSide(x)).ToList();

            return result;
        }

        public Line LineAfterHit(Line incoming, Vector3D hitPoint)
        {
            HitPoints.Add(hitPoint);
            var baseLine = new Line(TopCenter, BottomCenter);
            var line = new Line(baseLine.ClosestPoint(hitPoint), hitPoint);
            var normalVector = -1 * line.Direction.Normalize();

            var newDirection = 2 * (-1 * incoming.Direction.Normalize() * normalVector) * normalVector +
                               incoming.Direction.Normalize();
            return Line.FromPointAndDirection(hitPoint, newDirection);
        }

        public string ObjectName { get; set; }

        private bool InsideTheCylinder(Vector3D point)
        {
            var baseLine = new Line(TopCenter, BottomCenter);
            var projectedPoint = baseLine.ClosestPoint(point);

            var distanceFromTop = Vector3D.AbsoluteValue(projectedPoint - TopCenter);
            var distanceFromBottom = Vector3D.AbsoluteValue(projectedPoint - BottomCenter);

            return Math.Abs(distanceFromBottom + distanceFromTop - _cylinderHeight) < Confidence;
        }

        public override bool Equals(object obj)
        {
            if (obj is null) return false;
            if (ReferenceEquals(this, obj)) return true;
            return (obj.GetType() == GetType()) && Equals((Cylinder) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Radius.GetHashCode();
                hashCode = (hashCode * 397) ^ TopCenter.GetHashCode();
                hashCode = (hashCode * 397) ^ BottomCenter.GetHashCode();
                return hashCode;
            }
        }
    }
}