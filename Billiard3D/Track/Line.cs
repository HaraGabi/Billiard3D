using System;
using Billiard3D.VectorMath;
using JetBrains.Annotations;

namespace Billiard3D.Track
{
    internal class Line
    {
        public Line([NotNull] Vector3D pointA, [NotNull] Vector3D pointB)
        {
            PointB = pointB ?? throw new ArgumentNullException(nameof(pointB));
            PointA = pointA ?? throw new ArgumentNullException(nameof(pointA));
            Direction = PointB - PointA;
        }

        public Vector3D PointB { get; }
        public Vector3D PointA { get; }
        public Vector3D Direction { get; }

        public double DistanceFrom(Vector3D point)
        {
            if (point == null)
                throw new ArgumentNullException(nameof(point));

            var topValue = Math.Pow(Vector3D.AbsoluteValue(Vector3D.CrossProduct(Direction, PointA - point)), 2);
            var bottomValue = Math.Pow(Vector3D.AbsoluteValue(Direction), 2);
            var squareResult = topValue / bottomValue;
            var result = Math.Sqrt(squareResult);
            return result;
        }

        public Vector3D ClosestPoint([NotNull] Vector3D toThisReference)
        {
            if (toThisReference == null) throw new ArgumentNullException(nameof(toThisReference));

            var v = toThisReference - PointA;
            var t = v * Direction.Normalize();
            return PointA + t * Direction.Normalize();
        }

        public Vector3D GetPointOnLine(double distance) => PointA + distance * Direction.Normalize();
    }
}