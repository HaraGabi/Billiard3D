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

        public List<Vector3D> Corners { get; } = new List<Vector3D>(4);
        public List<Line> WallLines { get; } = new List<Line>(4);
        public Vector3D NormalVector { get; set; }

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

        public List<Vector3D> HitPoints { get; } = new List<Vector3D>(100_000);

        public IEnumerable<Vector3D> GetIntersectionPoints(in Line line)
        {
            if (Math.Abs(line.Direction * NormalVector) < Confidence)
            {
                // No Intersection
                return Enumerable.Empty<Vector3D>();
            }
            var distance = (Corners.First() - line.PointA) * NormalVector / (line.Direction * NormalVector);
            if (distance < Confidence) return Enumerable.Empty<Vector3D>();
            var hitPoint = new List<Vector3D> {line.PointA + distance * line.Direction};
            hitPoint = hitPoint.Where(OnTheWall).ToList();

            return hitPoint;
        }

        public Line LineAfterHit(in Line incoming, in Vector3D hitPoint)
        {
            HitPoints.Add(hitPoint);
            var newDirection = 2 * (-1 * incoming.Direction * NormalVector) * NormalVector +
                               incoming.Direction;
            return Line.FromPointAndDirection(hitPoint, newDirection);
        }

        public bool IsInCorrectPosition(Line ball)
        {
            return false;
        }

        public string ObjectName { get; set; }

        private bool OnTheWall(Vector3D hitPoint)
        {
            var maxX = Corners.Max(x => x.X);
            var maxY = Corners.Max(x => x.Y);
            var maxZ = Corners.Max(x => x.Z);

            var minX = Corners.Min(x => x.X);
            var minY = Corners.Min(x => x.Y);
            var minZ = Corners.Min(x => x.Z);

            var smallerThanMaxX = (hitPoint.X < maxX) || (Math.Abs(hitPoint.X - maxX) < Confidence);
            var smallerThanMaxY = (hitPoint.Y < maxY) || (Math.Abs(hitPoint.Y - maxY) < Confidence);
            var smallerThanMaxZ = (hitPoint.Z < maxZ) || (Math.Abs(hitPoint.Z - maxZ) < Confidence);

            var biggerThanMinX = (hitPoint.X > minX) || (Math.Abs(hitPoint.X - minX) < Confidence);
            var biggerThanMinY = (hitPoint.Y > minY) || (Math.Abs(hitPoint.Y - minY) < Confidence);
            var biggerThanMinZ = (hitPoint.Z > minZ) || (Math.Abs(hitPoint.Z - minZ) < Confidence);

            var forX = smallerThanMaxX && biggerThanMinX;
            var forY = smallerThanMaxY && biggerThanMinY;
            var forZ = smallerThanMaxZ && biggerThanMinZ;

            var inBetween = forX && forY && forZ;
            return inBetween;
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