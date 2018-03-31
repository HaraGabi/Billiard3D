using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Billiard3D.VectorMath;

namespace Billiard3D.Track
{
    internal readonly struct Line : IEquatable<Line>, IEnumerable<Vector3D>
    {
        public Vector3D SecondPoint { get; }
        public Vector3D BasePoint { get; }
        public Vector3D Direction { get; }

        public Line(Vector3D basePoint, Vector3D secondPoint)
        {
            SecondPoint = secondPoint;
            BasePoint = basePoint;
            Direction = (SecondPoint - BasePoint).Normalize();
        }

        public IEnumerator<Vector3D> GetEnumerator()
        {
            yield return BasePoint;
            yield return SecondPoint;
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(Line other) => Equals(SecondPoint, other.SecondPoint) && Equals(BasePoint, other.BasePoint) &&
                                          Equals(Direction, other.Direction);

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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Line FromPointAndDirection(Vector3D point, Vector3D direction)
        {
            var pointB = point + 1.26 * direction;
            return new Line(point, pointB);
        }

        /// <summary>
        ///     The given point's distance from the line
        /// </summary>
        /// <param name="point">The point.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">point</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public double DistanceFrom(Vector3D point)
        {
            var topValue = Math.Pow(Vector3D.AbsoluteValue(Vector3D.CrossProduct(Direction, BasePoint - point)), 2);
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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Vector3D ClosestPoint(Vector3D toThisReference)
        {
            var v = toThisReference - BasePoint;
            var t = v * Direction.Normalize();
            return BasePoint + t * Direction;
        }

        /// <summary>
        ///     Gets the point on line.
        /// </summary>
        /// <param name="distance">The distance.</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Vector3D GetPointOnLine(double distance) => BasePoint + distance * Direction;

        /// <summary>
        ///     Gets the point on line assuming the given reference is already on the line
        /// </summary>
        /// <param name="referencePoint">The reference point.</param>
        /// <param name="distance">The distance.</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Vector3D GetPointOnLine(Vector3D referencePoint, double distance) =>
            referencePoint + distance * Direction;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override bool Equals(object obj)
        {
            if (obj is null) return false;
            return obj.GetType() == GetType() && Equals((Line) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = SecondPoint.GetHashCode();
                hashCode = hashCode * 397 ^ BasePoint.GetHashCode();
                hashCode = hashCode * 397 ^ Direction.GetHashCode();
                return hashCode;
            }
        }
    }
}