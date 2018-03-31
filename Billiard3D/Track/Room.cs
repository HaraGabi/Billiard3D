using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Billiard3D.VectorMath;
using JetBrains.Annotations;

namespace Billiard3D.Track
{
    internal class Room
    {
        public List<Vector3D> EveryHitpoint { get; } = new List<Vector3D>();

        public List<string> HitSequence { get; } = new List<string>();

        public int NumberOfIterations { private get; set; } = 10_000;

        public List<ITrackBoundary> Boundaries { get; } = new List<ITrackBoundary>(24);

        public double Radius { get; }

        public Room([NotNull] IEnumerable<ITrackBoundary> bounds, double radius)
        {
            if (bounds == null) throw new ArgumentNullException(nameof(bounds));

            Boundaries.AddRange(bounds);
            Radius = radius;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Start(Line startLine)
        {
            var currentLine = startLine;
            foreach (var item in Boundaries)
            {
                if (item.IsInCorrectPosition(startLine))
                    throw new ArgumentException(item.BoundaryName);
            }

            for (var i = 0; i < NumberOfIterations; ++i)
            {
                List<(ITrackBoundary boundary, Vector3D intersectionPoint)> hitPoints = Boundaries
                    .Select(boundary => (boundary, boundary.GetIntersectionPoints(in currentLine)))
                    .Where(x => x.Item2.Any())
                    .Select(x =>
                        (x.boundary, x.Item2.OrderByDescending(v => Vector3D.AbsoluteValue(v - currentLine.BasePoint))
                            .First()))
                    .ToList();
                var hitted = hitPoints.OrderBy(x => Selector(x.boundary)).First();
                HitSequence.Add(hitted.boundary.BoundaryName);

                EveryHitpoint.Add(hitted.intersectionPoint);
                currentLine = hitted.boundary.LineAfterHit(in currentLine, in hitted.intersectionPoint);
            }
        }

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Selector(ITrackBoundary trackBoundary)
        {
            switch (trackBoundary)
            {
                case Sphere _: return 0;
                case Cylinder _: return 1;
                case Wall _: return 2;
                default: throw new NotSupportedException();
            }
        }
    }
}