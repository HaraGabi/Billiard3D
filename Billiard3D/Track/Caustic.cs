using System.Collections.Generic;
using System.Linq;
using Billiard3D.VectorMath;

namespace Billiard3D.Track
{
    internal class Caustic
    {
        private List<ITrackBoundary> TrackObjects { get; } = new List<ITrackBoundary>();

        public Caustic()
        {
            const int dimension = 100;
            const int half = dimension / 2;
            var frontWall = new Wall(new Vector3D[]
                {(0, 0, 0), (0, dimension, 0), (0, dimension, dimension), (0, 0, dimension)});
            var backWall = new Wall(new Vector3D[]
            {
                (dimension, 0, 0), (dimension, dimension, 0), (dimension, dimension, dimension),
                (dimension, 0, dimension)
            });
            var rightWall = new Wall(new Vector3D[]
                {(0, 0, 0), (dimension, 0, 0), (dimension, 0, dimension), (0, 0, dimension)});
            var leftWall = new Wall(new Vector3D[]
            {
                (0, dimension, 0), (dimension, dimension, 0), (dimension, dimension, dimension),
                (0, dimension, dimension)
            });
            var floor = new Wall(new Vector3D[]
                {(0, 0, 0), (0, dimension, 0), (dimension, dimension, 0), (dimension, 0, 0)});
            var ceiling = new Wall(new Vector3D[]
            {
                (0, 0, dimension), (0, dimension, dimension), (dimension, dimension, dimension),
                (dimension, 0, dimension)
            });

            var sphere = new Sphere((half, half, half), half);
            var plane = new Plane((half, dimension, half), (half, half, 0), (half, 0, half));
            var sign = plane.DeterminePointPosition((0, half, half));
            sphere.Checker = new PointChecker(plane, sign);
            TrackObjects.AddRange(new ITrackBoundary[]
                {frontWall, backWall, rightWall, leftWall, floor, ceiling, sphere});
        }

        public List<Vector3D> Start(Line startLine)
        {
            var currentLine = startLine;
            var hitSequence = new List<Vector3D>();
            for (var i = 0; i < 2; ++i)
            {
                var hitPoints = TrackObjects.Select(x => (x, x.GetIntersectionPoints(in currentLine)))
                    .Where(x => x.Item2.Any())
                    .Select(x =>
                        (x.x, x.Item2.OrderByDescending(v => Vector3D.AbsoluteValue(v - currentLine.BasePoint)).First()))
                    .ToList();
                var hitted = hitPoints.OrderBy(x => Room.Selector(x.x)).First();

                if (hitted.x is Wall) return Enumerable.Empty<Vector3D>().ToList();
                hitSequence.Add(hitted.Item2);

                currentLine = hitted.x.LineAfterHit(in currentLine, in hitted.Item2);
            }

            return hitSequence;
        }
    }
}