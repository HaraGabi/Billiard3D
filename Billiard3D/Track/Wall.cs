using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Billiard3D.VectorMath;
using static Billiard3D.VectorMath.Vector3D;

namespace Billiard3D.Track
{
    /// <summary>
    ///     We require the points to be given in order so no two points are given one after the other that are on the opposite
    ///     of each other
    /// </summary>
    [DebuggerDisplay("({NormalVector.X}, {NormalVector.Y}, {NormalVector.Z})")]
    internal class Wall : ITrackObject
    {
        private const double Confidence = 0.00005;

        public Wall(IEnumerable<Vector3D> corners)
        {
            Corners.AddRange(corners);
            NormalVector = CrossProduct(Corners[1] - Corners[0], Corners[2] - Corners[0]).Normalize();
            if (!CheckIfPointIsOnThePlain(Corners.Last()))
                throw new ArgumentException();
            foreach (var corner in Corners)
            {
                var index = Corners.IndexOf(corner);
                var nextIndex = index + 1 < Corners.Count ? index + 1 : 0;
                WallLines.Add(new Line(corner, Corners[nextIndex]));
            }
        }

        public List<Vector3D> Corners { get; set; } = new List<Vector3D>();
        public List<Line> WallLines { get; } = new List<Line>(4);
        public Vector3D NormalVector { get; set; }
        public List<Vector3D> HittedPoints { get; set; } = new List<Vector3D>();


        public IEnumerable<Vector3D> GetIntersectionPoints(Line line)
        {
            if (line.Direction * NormalVector < Confidence)
            {
                // No Intersection
                yield break;
            }
            var distance = (Corners.First() - line.PointA) * NormalVector / (line.Direction * NormalVector);
            yield return line.PointA + distance * line.Direction;
        }

        public Line LineAfterHit(Line incoming, Vector3D hittedPoint)
        {
            HittedPoints.Add(hittedPoint);
            var newDirection = 2 * (-1 * incoming.Direction.Normalize() * NormalVector) * NormalVector +
                               incoming.Direction.Normalize();
            return Line.FromPointAndDirection(hittedPoint, newDirection);
        }

        private bool CheckIfPointIsOnThePlain((double x, double y, double z) point)
        {
            const double confidence = 0.000001;
            var forX = NormalVector.X * (point.x - Corners.First().X);
            var forY = NormalVector.Y * (point.y - Corners.First().Y);
            var forZ = NormalVector.Z * (point.z - Corners.First().Z);
            return forX + forY + forZ <= confidence;
        }

        public double NormalEquation(Vector3D tuple)
        {
            var ax = NormalVector.X * tuple.X;
            var by = NormalVector.Y * tuple.Y;
            var cz = NormalVector.Z * tuple.Z;
            var d = NormalVector.X * Corners.First().X + NormalVector.Y * Corners.First().Y +
                    NormalVector.Z * Corners.First().Z;
            return ax + by + cz - d;
        }

        public bool WasHit(Vector3D hitPoint) => CheckIfPointIsOnThePlain(hitPoint);
    }
}