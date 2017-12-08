using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Billiard3D.VectorMath;
using JetBrains.Annotations;

namespace Billiard3D.Track
{
    internal class Cylinder : ITrackObject, IEquatable<Cylinder>, IEnumerable<Vector3D>
    {
        private double Radius { get; }

        public Vector3D TopCenter { get; }

        public Vector3D BottomCenter { get; }

        public Cylinder([NotNull] Vector3D top, [NotNull] Vector3D bottom, double radius)
        {
            Radius = radius;
            TopCenter = top ?? throw new ArgumentNullException(nameof(top));
            BottomCenter = bottom ?? throw new ArgumentNullException(nameof(bottom));
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

        public List<Vector3D> HitPoints { get; } = new List<Vector3D>(10_000_000);

        /// <summary>
        ///     Gets the necessary coefficient for the line to reach cylinder
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        public (IEnumerable<(Vector3D, double)>, ITrackObject) GetIntersectionPoints(Line line)
        {
            if (line == null) throw new ArgumentNullException(nameof(line));

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

            var result = new List<(Vector3D, double)>
                {
                    (line.PointA + positive * line.Direction, positive),
                    (line.PointA + negative * line.Direction, negative)
                }.Where(x => x.Item2 > 0.00005).Where(x => InsideTheCylinder(x.Item1))
                .ToList();

            return (result, this);
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

            var cylinderHeight = Vector3D.AbsoluteValue(BottomCenter - TopCenter);
            var distanceFromTop = Vector3D.AbsoluteValue(projectedPoint - TopCenter);
            var distanceFromBottom = Vector3D.AbsoluteValue(projectedPoint - BottomCenter);

            return Math.Abs(distanceFromBottom + distanceFromTop - cylinderHeight) < 0.00005;
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
                hashCode = (hashCode * 397) ^ (TopCenter != null ? TopCenter.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (BottomCenter != null ? BottomCenter.GetHashCode() : 0);
                return hashCode;
            }
        }
    }
}