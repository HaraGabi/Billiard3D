using System;
using System.Collections.Generic;
using Billiard3D.VectorMath;
using JetBrains.Annotations;

namespace Billiard3D.Track
{
    internal class Cylinder : ITrackObject
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

        /// <summary>
        ///     Gets the necessary coefficient for the line to reach cylinder
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        public IEnumerable<Vector3D> GetIntersectionPoints(Line line)
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

            var firstValue = (-b + Math.Sqrt(Math.Pow(b, 2) + 4 * a * c)) / 2 * a;
            var secondValue = (-b - Math.Sqrt(Math.Pow(b, 2) + 4 * a * c)) / 2 * a;

            yield return line.PointA + firstValue * line.Direction;
            yield return line.PointA + secondValue * line.Direction;
        }

        public Line LineAfterHit(Line incoming, Vector3D hittedPoint)
        {
            HittedPoints.Add(hittedPoint);
            var baseLine = new Line(TopCenter, BottomCenter);
            var line = new Line(baseLine.ClosestPoint(hittedPoint), hittedPoint);
            var normalVector = line.Direction.Normalize();

            var newDirection = 2 * (-1 * incoming.Direction.Normalize() * normalVector) * normalVector +
                               incoming.Direction.Normalize();
            return Line.FromPointAndDirection(hittedPoint, newDirection);
        }
    }
}