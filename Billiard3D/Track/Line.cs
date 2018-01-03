using System;
using System.Collections;
using System.Collections.Generic;
using Billiard3D.VectorMath;
using JetBrains.Annotations;

namespace Billiard3D.Track
{
    internal struct Line : IEquatable<Line>, IEnumerable<Vector3D>
    {
        public Vector3D PointB { get; }
        public Vector3D PointA { get; }
        public Vector3D Direction { get; }

        public Line(Vector3D pointA, Vector3D pointB)
        {
            PointB = pointB;
            PointA = pointA;
            Direction = (PointB - PointA).Normalize();
        }

        public IEnumerator<Vector3D> GetEnumerator()
        {
            yield return PointA;
            yield return PointB;
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public bool Equals(Line other)
        {
            // todo: this is not true
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
        public double DistanceFrom(Vector3D point)
        {
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
        public Vector3D ClosestPoint(Vector3D toThisReference)
        {
            var v = toThisReference - PointA;
            var t = v * Direction.Normalize();
            return PointA + t * Direction;
        }

        /// <summary>
        ///     Gets the point on line.
        /// </summary>
        /// <param name="distance">The distance.</param>
        /// <returns></returns>
        public Vector3D GetPointOnLine(double distance) => PointA + distance * Direction;

        /// <summary>
        ///     Gets the point on line assuming the given reference is already on the line
        /// </summary>
        /// <param name="referencePoint">The reference point.</param>
        /// <param name="distance">The distance.</param>
        /// <returns></returns>
        public Vector3D GetPointOnLine(Vector3D referencePoint, double distance) =>
            referencePoint + distance * Direction;

        public override bool Equals(object obj)
        {
            if (obj is null) return false;
            return (obj.GetType() == GetType()) && Equals((Line) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = PointB.GetHashCode();
                hashCode = (hashCode * 397) ^  PointA.GetHashCode();
                hashCode = (hashCode * 397) ^  Direction.GetHashCode();
                return hashCode;
            }
        }
    }
}