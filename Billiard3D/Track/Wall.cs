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


        public (IEnumerable<(Vector3D, double)>, ITrackObject) GetIntersectionPoints(Line line, DiscardMode discardMode)
        {
            if (Math.Abs(line.Direction * NormalVector) < Confidence)
            {
                // No Intersection
                return (Enumerable.Empty<(Vector3D, double)>(), this);
            }
            // todo: specifics
            var distance = (Corners.First() - line.PointA) * NormalVector / (line.Direction * NormalVector);
            if ((discardMode == DiscardMode.DiscardBackWards) && (distance < 0))
                return (Enumerable.Empty<(Vector3D, double)>(), this);
            return (new[] {(line.PointA + distance * line.Direction, distance)}, this);
        }

        public Line LineAfterHit(Line incoming, Vector3D hittedPoint)
        {
            HittedPoints.Add(hittedPoint);
            // todo: specifics
            var newDirection = 2 * (-1 * incoming.Direction.Normalize() * NormalVector) * NormalVector +
                               incoming.Direction.Normalize();
            return Line.FromPointAndDirection(hittedPoint, newDirection);
        }

        private bool CheckIfPointIsOnThePlain((double x, double y, double z) point)
        {
            const double confidence = 0.000001;
            var forX = Math.Abs(NormalVector.X * (point.x - Corners.First().X)) <= confidence;
            var forY = Math.Abs(NormalVector.Y * (point.y - Corners.First().Y)) <= confidence;
            var forZ = Math.Abs(NormalVector.Z * (point.z - Corners.First().Z)) <= confidence;
            return forX && forY && forZ;
        }
    }
}