using System;
using System.Collections.Generic;
using System.Linq;
using Billiard3D.VectorMath;
using JetBrains.Annotations;

namespace Billiard3D.Track
{
    internal class Room
    {
        public Room([NotNull] IEnumerable<Wall> walls, double radius = 0.003)
        {
            if (walls == null) throw new ArgumentNullException(nameof(walls));
            Radius = radius;
            Objects.AddRange(walls);
            CreateCylinders(radius);
            CreateSpheres(radius);
        }

        public double Radius { get; }

        public List<ITrackObject> Objects { get; set; } = new List<ITrackObject>();

        public int NumberOfIterations { get; set; } = 10_000;

        private double MinimumWallDistance { get; set; }

        public List<ITrackObject> Start(Line startLine)
        {
            var currentLine = startLine;
            var previous = new List<ITrackObject>();
            for (var i = 0; i < NumberOfIterations; i++)
            {
                var hitPoints = Objects.Select(x => x.GetIntersectionPoints(currentLine)).Where(x => x.Item1.Any()).ToList();
                var hittedWall = hitPoints.Where(x => x.Item2 is Wall && x.Item1.Any()).OrderBy(x => x.Item1.Min(y => y.Item2)).FirstOrDefault();

                var hitPoint = hittedWall.Item1?.First().Item1;
                var wall = hittedWall.Item2 as Wall;
                if (hitPoint is null || (wall?.WallLines.Any(x => x.DistanceFrom(hitPoint) < MinimumWallDistance) ?? false))
                {
                    var hittedSphere = hitPoints.FirstOrDefault(x => x.Item2 is Sphere);
                    if (hittedSphere.Item2 is null)
                    {
                        // no sphere was hit
                        var hittedCylinder = hitPoints.Where(x => x.Item2 is Cylinder).First(x => x.Item1.Any());
                        var cylinderHitPoint = hittedCylinder.Item1.OrderBy(x => x.Item2).Last();
                        currentLine = hittedCylinder.Item2.LineAfterHit(currentLine, cylinderHitPoint.Item1);
                        previous.Clear();
                        previous.Add(hittedCylinder.Item2);
                    }
                    else
                    {
                        var sphereHitPoint = hittedSphere.Item1.OrderBy(x => x.Item2).Last();
                        currentLine = hittedSphere.Item2.LineAfterHit(currentLine, sphereHitPoint.Item1);
                        previous.Clear();
                        previous.Add(hittedSphere.Item2);
                    }
                }
                else
                {
                    // wall was hit
                    currentLine = wall.LineAfterHit(currentLine, hitPoint);
                    previous.Clear();
                    previous.Add(wall);
                }
            }
            return Objects;
        }

        private void CreateCylinders(double radius)
        {
            var cylinders = new List<Cylinder>();
            foreach (var track in Objects)
            {
                var wall = track as Wall ?? throw new ArgumentNullException(nameof(Objects));
                foreach (var wallLine in wall.WallLines)
                {
                    var otherWall = Objects.Except(new[] {wall}).Cast<Wall>().ToList().Single(x =>
                        x.WallLines.Exists(line =>
                            ((line.PointA == wallLine.PointA) && (line.PointB == wallLine.PointB)) ||
                            ((line.PointA == wallLine.PointB) && (line.PointB == wallLine.PointA))));

                    var topLine = wall.WallLines.Single(line =>
                        ((line.PointA == wallLine.PointA) && (line.PointB != wallLine.PointB)) ||
                        ((line.PointA == wallLine.PointB)
                         && (line.PointB != wallLine.PointA)));

                    var otherTopLine = otherWall.WallLines
                        .Except(new[] {new Line(wallLine.PointB, wallLine.PointA), wallLine})
                        .Single(x =>
                            (x.PointA == topLine.PointA) || (x.PointA == topLine.PointB) ||
                            (x.PointB == topLine.PointA) ||
                            (x.PointB == topLine.PointB));

                    var cylinder = CalculateCylinder(topLine, otherTopLine, wallLine, radius);
                    if (cylinders.Exists(x => x.Contains(cylinder.TopCenter) && x.Contains(cylinder.BottomCenter)))
                        continue;
                    cylinders.Add(cylinder);
                }
            }
            Objects.AddRange(cylinders);
        }

        private void CreateSpheres(double radius)
        {
            var cylinders = Objects.Where(x => x is Cylinder).Cast<Cylinder>();
            var spheres = new List<Sphere>();
            foreach (var cylinder in cylinders)
            {
                var lineA = new Line(cylinder.TopCenter, cylinder.BottomCenter);
                var lineB = new Line(cylinder.BottomCenter, cylinder.TopCenter);

                var sphereA = new Sphere(lineA.GetPointOnLine(radius), radius);
                var sphereB = new Sphere(lineB.GetPointOnLine(radius), radius);
                if (!spheres.Exists(x => x.Center == sphereA.Center))
                    spheres.Add(sphereA);
                if (!spheres.Exists(x => x.Center == sphereB.Center))
                    spheres.Add(sphereB);
            }
            Objects.AddRange(spheres);
        }

        private Cylinder CalculateCylinder([NotNull] Line first, [NotNull] Line second, [NotNull] Line wallLine,
            double radius)
        {
            if (first == null) throw new ArgumentNullException(nameof(first));
            if (second == null) throw new ArgumentNullException(nameof(second));
            if (wallLine == null) throw new ArgumentNullException(nameof(wallLine));

            var angle = Vector3D.Angle(first.Direction, second.Direction);
            var distance = Math.Sin(90.0.ToRadian()) / Math.Sin(angle / 2) * radius;
            MinimumWallDistance = Math.Sqrt(Math.Pow(distance, 2) - Math.Pow(radius, 2));

            var referencePoint = wallLine.Contains(first.PointA) ? first.PointA : first.PointB;
            var firstSign = first.PointA == referencePoint ? +1 : -1;
            var secondSign = second.PointA == referencePoint ? +1 : -1;

            var firstPoint = first.GetPointOnLine(referencePoint, firstSign * MinimumWallDistance);
            var secondPoint = second.GetPointOnLine(referencePoint, secondSign * MinimumWallDistance);

            var line = new Line(firstPoint, secondPoint);
            var directLine = new Line(referencePoint, line.ClosestPoint(referencePoint));
            var top = directLine.GetPointOnLine(distance);

            var difference = top - referencePoint;
            var otherPoint = referencePoint == wallLine.PointA ? wallLine.PointB : wallLine.PointA;
            var bottom = otherPoint + difference;

            return new Cylinder(top, bottom, radius);
        }
    }
}