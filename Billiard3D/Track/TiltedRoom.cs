using System;
using System.Collections.Generic;
using System.Linq;
using Billiard3D.VectorMath;
using JetBrains.Annotations;

namespace Billiard3D.Track
{
    internal class TiltedRoom
    {
        private const double Confidence = 0.00005;

        public TiltedRoom([NotNull] IEnumerable<Wall> walls, double radius)
        {
            if (walls == null) throw new ArgumentNullException(nameof(walls));
            Radius = radius;

            foreach (var wall in walls)
            {
                wall.ObjectName = "Wall" + wall.NormalVector;
                Objects.Add(wall);
            }
            CreateRadiusObjects();
        }

        public double Radius { get; }
        public List<ITrackObject> Objects { get; } = new List<ITrackObject>();

        public List<string> HitSequence { get; } = new List<string>(10_000_000);

        public int NumberOfIterations { get; set; } = 10_000_000;

        public void Start(Line startLine)
        {
            var currentLine = startLine;
            for (var i = 0; i < NumberOfIterations; i++)
            {
                var hitPoints = Objects.Select(x => x.GetIntersectionPoints(currentLine)).Where(x => x.Item1.Any())
                    .ToList();
                var hittedWall = hitPoints.Where(x => x.Item2 is Wall)
                    .OrderBy(x => x.Item1.First().Item2).FirstOrDefault();

                var hitPoint = hittedWall.Item1?.First().Item1;
                var wall = hittedWall.Item2 as Wall;
                ITrackObject previous;
                if (wall?.WallLines.Any(x => WasWallHit(x, hitPoint, wall)) ?? true)
                {
                    var hittedSphere = hitPoints.FirstOrDefault(x => x.Item2 is Sphere);
                    if (hittedSphere.Item2 is null)
                    {
                        // no sphere was hit
                        var hittedCylinder = hitPoints.First(x => x.Item2 is Cylinder);
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
                    currentLine = wall.LineAfterHit(currentLine, hitPoint);
                    previous = wall;
                }
                HitSequence.Add(previous.ObjectName);
            }
        }

        private static bool CheckPrevious(ITrackObject previous, (IEnumerable<(Vector3D, double)>, ITrackObject) hittedSphere2)
        {
            switch (previous)
            {
                case null:
                    return false;
                case Cylinder _:
                case Sphere _:
                    return hittedSphere2.Item1?.Count() > 1;
            }
            return true;
        }

        private bool WasWallHit(Line borderLine, Vector3D hitPoint, Wall wall)
        {
            var distanceFrom = borderLine.DistanceFrom(hitPoint);
            var min = MinimumWallDistance(borderLine, wall);
            var smaller = distanceFrom < min;
            var equals = Math.Abs(distanceFrom - min) < Confidence;
            return smaller && !equals;
        }

        private double MinimumWallDistance(Line borderLine, Wall wall)
        {
            var otherWall = Objects.Where(x => x is Wall).Except(new[] { wall }).Cast<Wall>().ToList().Single(x =>
                x.WallLines.Exists(line =>
                    ((line.PointA == borderLine.PointA) && (line.PointB == borderLine.PointB)) ||
                    ((line.PointA == borderLine.PointB) && (line.PointB == borderLine.PointA))));
            var angle = Math.Acos(Math.Abs(wall.NormalVector * otherWall.NormalVector) /
                                  (Vector3D.AbsoluteValue(wall.NormalVector) * Vector3D.AbsoluteValue(otherWall.NormalVector)));
            if (otherWall.NormalVector == (1, 0, 0))
            {
                angle = 180.0.ToRadian() - angle;
            }
            var distance = Math.Sin(90.0.ToRadian()) / Math.Sin(angle / 2) * Radius;
            distance = Math.Sqrt(Math.Pow(distance, 2) - Math.Pow(Radius, 2));
            return distance;
        }

        private void CreateRadiusObjects()
        {
            var floor = Objects.Where(x => x is Wall).Cast<Wall>().Single(x => x.NormalVector == (0, 0, 1));
            var firstLine = floor.WallLines.Single(x => (x.PointB == (0, 0, 0)) && (x.PointA == (0, 640, 0)));
            var secondLine = floor.WallLines.Single(x => (x.PointA == (975, 0, 0)) && (x.PointB == (975, 640, 0)));
            firstLine = new Line(firstLine.PointB, firstLine.PointA);

            var firstBarrierLine = new Line(firstLine.GetPointOnLine(Radius), secondLine.GetPointOnLine(Radius));
            var secondBarrierLine = new Line(
                firstLine.GetPointOnLine(Vector3D.AbsoluteValue(firstLine.PointA - firstLine.PointB) - Radius),
                secondLine.GetPointOnLine(Vector3D.AbsoluteValue(secondLine.PointA - secondLine.PointB) - Radius));

            var firstCenter = firstBarrierLine.GetPointOnLine(Radius) + (0, 0, Radius);
            var secondCenter = firstBarrierLine.GetPointOnLine(975 - Radius) + (0, 0, Radius);
            var thirdCenter = secondBarrierLine.GetPointOnLine(Radius) + (0, 0, Radius);
            var fourthCenter = secondBarrierLine.GetPointOnLine(975 - Radius) + (0, 0, Radius);

            var (a1, a2) = GetAValues();

            var topFirstCenter = firstCenter - (0, 0, 2 * Radius) + (0, 0, 215) + (0, 0, a1);
            var topSecondCenter = secondCenter - (0, 0, 2 * Radius) + (0, 0, 215) + (0, 0, a2);
            var topThirdCenter = thirdCenter - (0, 0, 2 * Radius) + (0, 0, 215) + (0, 0, a1);
            var topFourthCenter = fourthCenter - (0, 0, 2 * Radius) + (0, 0, 215) + (0, 0, a2);

            var centers = new[]
            {
                firstCenter, secondCenter, thirdCenter, fourthCenter, topFirstCenter, topSecondCenter, topThirdCenter,
                topFourthCenter
            };
            Objects.AddRange(centers.Select(x => new Sphere(x, Radius) {ObjectName = "Sphere" + $" Center {x}"}));

            var c1 = new Cylinder(firstCenter, secondCenter, Radius);
            var c2 = new Cylinder(firstCenter, thirdCenter, Radius);
            var c3 = new Cylinder(firstCenter, topFirstCenter, Radius);
            var c4 = new Cylinder(secondCenter, fourthCenter, Radius);
            var c5 = new Cylinder(secondCenter, topSecondCenter, Radius);
            var c6 = new Cylinder(thirdCenter, fourthCenter, Radius);
            var c7 = new Cylinder(thirdCenter, topThirdCenter, Radius);
            var c8 = new Cylinder(fourthCenter, topFourthCenter, Radius);
            var c9 = new Cylinder(topFirstCenter, topSecondCenter, Radius);
            var c10 = new Cylinder(topFirstCenter, topThirdCenter, Radius);
            var c11 = new Cylinder(topSecondCenter, topFourthCenter, Radius);
            var c12 = new Cylinder(topThirdCenter, topFourthCenter, Radius);
            Objects.AddRange(new[] {c1, c2, c3, c4, c5, c6, c7, c8, c9, c10, c11, c12});
            foreach (Cylinder cylinder in Objects.Where(x => x is Cylinder))
            {
                cylinder.ObjectName = "Cylinder" + $" Top {cylinder.TopCenter}" + $" Bottom {cylinder.BottomCenter}";
            }
        }

        private (double a1, double a2) GetAValues()
        {
            var c = Math.Sqrt(975 * 975 + 442 * 442);
            var sinAlpha = 442 / c;
            var sinBeta = 975 / c;

            var a1 = sinAlpha / sinBeta * Radius;
            var a2 = sinAlpha / sinBeta * (975 - Radius);

            return (a1, a2);
        }
    }
}