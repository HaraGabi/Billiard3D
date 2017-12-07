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
        public Cylinder([NotNull] Vector3D top, [NotNull] Vector3D bottom, double radius)
        {
            Radius = radius;
            TopCenter = top ?? throw new ArgumentNullException(nameof(top));
            BottomCenter = bottom ?? throw new ArgumentNullException(nameof(bottom));
        }

        public double Radius { get; }

        public Vector3D TopCenter { get; }

        public Vector3D BottomCenter { get; }

        public List<Vector3D> HittedPoints { get; } = new List<Vector3D>();

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

        /// <summary>
        ///     Gets the necessary coefficient for the line to reach cylinder
        /// </summary>
        /// <param name="line"></param>
        /// <param name="discardMode"></param>
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

            var positive = (-b + Math.Sqrt(Math.Pow(b, 2) + 4 * a * c)) / 2 * a;
            var negative = (-b - Math.Sqrt(Math.Pow(b, 2) + 4 * a * c)) / 2 * a;

            var result = new List<(Vector3D, double)>
            {
                (line.PointA + positive * line.Direction, positive),
                (line.PointA + negative * line.Direction, negative)
            };

            return (result, this);
        }

        public Line LineAfterHit(Line incoming, Vector3D hittedPoint)
        {
            HittedPoints.Add(hittedPoint);
            var baseLine = new Line(TopCenter, BottomCenter);
            var line = new Line(baseLine.ClosestPoint(hittedPoint), hittedPoint);
            var normalVector = - 1 * line.Direction.Normalize();

            var newDirection = 2 * (-1 * incoming.Direction.Normalize() * normalVector) * normalVector +
                               incoming.Direction.Normalize();
            return Line.FromPointAndDirection(hittedPoint, newDirection);
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