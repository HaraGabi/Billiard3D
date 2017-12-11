using System;
using System.Collections.Generic;
using System.Linq;
using Billiard3D.VectorMath;
using JetBrains.Annotations;

namespace Billiard3D.Track
{
    internal class Room
    {
        private const double Confidence = 0.00005;
        public double Radius { get; }

        public List<ITrackObject> Objects { get; } = new List<ITrackObject>(24);

        private int NumberOfIterations { get; } = 10_000_000;

        public List<string> HitSequence { get; } = new List<string>(10_000_000);

        private double MinimumWallDistance { get; set; }

        public Room([NotNull] IEnumerable<Wall> walls, double radius = 0.003)
        {
            if (walls == null) throw new ArgumentNullException(nameof(walls));
            Radius = radius;
            foreach (var wall in walls)
            {
                wall.ObjectName = "Wall" + wall.NormalVector;
                Objects.Add(wall);
            }
            CreateCylinders(radius);
            CreateSpheres(radius);
        }

        public void Start(Line startLine)
        {
            var currentLine = startLine;
            for (var i = 0; i < NumberOfIterations; i++)
            {
                var hitPoints = Objects.Select(x => x.GetIntersectionPoints(currentLine)).Where(x => x.Item1.Any())
                    .ToList();
                var hittedWall = hitPoints.Where(x => x.Item2 is Wall).OrderBy(x => x.Item1.Min(y => y.Item2)).First();

                var hitPoint = hittedWall.Item1.First().Item1;
                var wall = (Wall)hittedWall.Item2;
                ITrackObject previous;
                if (wall.WallLines.Any(x => WasWallHit(x, hitPoint)))
                {
                    var hittedSphere = hitPoints.FirstOrDefault(x => x.Item2 is Sphere);
                    if (hittedSphere.Item2 is null)
                    {
                        // no sphere was hit
                        var hittedCylinder = hitPoints.Where(x => x.Item2 is Cylinder).First(x => x.Item1.Any());
                        var cylinderHitPoint = hittedCylinder.Item1.OrderBy(x => x.Item2).Last();
                        currentLine = hittedCylinder.Item2.LineAfterHit(currentLine, cylinderHitPoint.Item1);
                        previous = hittedCylinder.Item2;
                    }
                    else
                    {
                        var sphereHitPoint = hittedSphere.Item1.OrderBy(x => x.Item2).Last();
                        currentLine = hittedSphere.Item2.LineAfterHit(currentLine, sphereHitPoint.Item1);
                        previous = hittedSphere.Item2;
                    }
                }
                else
                {
                    // wall was hit
                    currentLine = wall?.LineAfterHit(currentLine, hitPoint) ?? throw new InvalidOperationException();
                    previous = wall;
                }
                HitSequence.Add(previous.ObjectName);
            }
        }

        private bool WasWallHit(Line borderLine, Vector3D hitPoint)
        {
            var distanceFrom = borderLine.DistanceFrom(hitPoint);
            var smaller = distanceFrom < MinimumWallDistance;
            var equals = Math.Abs(distanceFrom - MinimumWallDistance) < Confidence;
            return smaller && !equals;
        }

        private void CreateCylinders(double radius)
        {
            var cylinders = new List<Cylinder>(12);
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
                    cylinder.ObjectName =
                        "Cylinder" + $" Top {cylinder.TopCenter}" + $" Bottom {cylinder.BottomCenter}";
                    cylinders.Add(cylinder);
                }
            }
            Objects.AddRange(cylinders);
        }

        private void CreateSpheres(double radius)
        {
            var cylinders = Objects.Where(x => x is Cylinder).Cast<Cylinder>();
            var spheres = new List<Sphere>(8);
            foreach (var cylinder in cylinders)
            {
                var sphereA = new Sphere(cylinder.TopCenter, radius);
                var sphereB = new Sphere(cylinder.BottomCenter, radius);
                if (!spheres.Exists(x => x.Center == sphereA.Center))
                {
                    sphereA.ObjectName = "Sphere" + $" Center {sphereA.Center}";
                    spheres.Add(sphereA);
                }

                if (!spheres.Exists(x => x.Center == sphereB.Center))
                {
                    sphereB.ObjectName = "Sphere" + $" Center {sphereB.Center}";
                    spheres.Add(sphereB);
                }
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

            var basLine = new Line(top, bottom);
            var reverseBase = new Line(bottom, top);

            var correctTop = basLine.GetPointOnLine(radius);
            var correctBottom = reverseBase.GetPointOnLine(radius);

            return new Cylinder(correctTop, correctBottom, radius);
        }
    }
}