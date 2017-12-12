using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace Billiard3D.Track
{
    internal class PureRoom
    {
        public PureRoom([NotNull] IEnumerable<Wall> walls)
        {
            foreach (var wall in walls)
            {
                wall.ObjectName = "Wall" + wall.NormalVector;
                Walls.Add(wall);
            }
        }

        public List<Wall> Walls { get; } = new List<Wall>(6);

        public List<string> Sequence { get; } = new List<string>(10_000);

        public void Start(Line startingPoint)
        {
            var currentLine = startingPoint;
            for (var i = 0; i < 10_000_000; ++i)
            {
                var hittedPoints = Walls.Select(x => x.GetIntersectionPoints(currentLine)).Where(x => x.Item1.Any())
                    .ToList();
                if (hittedPoints.Count != 1)
                    break;
                var hittedPoint = hittedPoints.Single();
                Sequence.Add(hittedPoint.Item2.ObjectName);
                currentLine = hittedPoint.Item2.LineAfterHit(currentLine,
                    hittedPoint.Item1.OrderByDescending(x => x.Item2).First().Item1);
            }
        }
    }
}