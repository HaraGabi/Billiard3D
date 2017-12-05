using System;
using System.Collections;
using System.Collections.Generic;
using Billiard3D.VectorMath;
using JetBrains.Annotations;

namespace Billiard3D.Track
{
    internal class Line : IEquatable<Line>, IEnumerable<Vector3D>
    {
        public Line([NotNull] Vector3D pointA, [NotNull] Vector3D pointB)
        {
            PointB = pointB ?? throw new ArgumentNullException(nameof(pointB));
            PointA = pointA ?? throw new ArgumentNullException(nameof(pointA));
            Direction = (PointB - PointA).Normalize();
        }

        public Vector3D PointB { get; }
        public Vector3D PointA { get; }
        public Vector3D Direction { get; }

        public IEnumerator<Vector3D> GetEnumerator()
        {
            yield return PointA;
            yield return PointB;
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public bool Equals(Line other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(PointB, other.PointB) && Equals(PointA, other.PointA) && Equals(Direction, other.Direction);
        }

        /// <summary>
        ///     Creates a <see cref="Line" /> using one point and a direction vector
        /// </summary>
        /// <param name="point">The point.</param>
        /// <param name="direction">The direction.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">
        ///     point
        ///     or
        ///     direction
        /// </exception>
        public static Line FromPointAndDirection([NotNull] Vector3D point, [NotNull] Vector3D direction)
        {
            if (point == null) throw new ArgumentNullException(nameof(point));
            if (direction == null) throw new ArgumentNullException(nameof(direction));

            var pointB = point + 1.26 * direction;
            return new Line(point, pointB);
        }

        /// <summary>
        ///     The given point's distance from the line
        /// </summary>
        /// <param name="point">The point.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">point</exception>
        public double DistanceFrom([NotNull] Vector3D point)
        {
            if (point == null)
                throw new ArgumentNullException(nameof(point));

            var topValue = Math.Pow(Vector3D.AbsoluteValue(Vector3D.CrossProduct(Direction, PointA - point)), 2);
            var bottomValue = Math.Pow(Vector3D.AbsoluteValue(Direction), 2);
            var squareResult = topValue / bottomValue;
            var result = Math.Sqrt(squareResult);
            return result;
        }

        /// <summary>
        ///     Closest point in the line to the point.
        /// </summary>
        /// <param name="toThisReference">To this reference.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">toThisReference</exception>
        public Vector3D ClosestPoint([NotNull] Vector3D toThisReference)
        {
            if (toThisReference == null) throw new ArgumentNullException(nameof(toThisReference));

            var v = toThisReference - PointA;
            var t = v * Direction.Normalize();
            return PointA + t * Direction.Normalize();
        }

        /// <summary>
        ///     Gets the point on line.
        /// </summary>
        /// <param name="distance">The distance.</param>
        /// <returns></returns>
        public Vector3D GetPointOnLine(double distance) => PointA + distance * Direction.Normalize();

        public override bool Equals(object obj)
        {
            if (obj is null) return false;
            if (ReferenceEquals(this, obj)) return true;
            return (obj.GetType() == GetType()) && Equals((Line) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = PointB != null ? PointB.GetHashCode() : 0;
                hashCode = (hashCode * 397) ^ (PointA != null ? PointA.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Direction != null ? Direction.GetHashCode() : 0);
                return hashCode;
            }
        }
    }
}