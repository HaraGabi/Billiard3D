using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Billiard3D.Track;
using Billiard3D.VectorMath;
using JetBrains.Annotations;

namespace Billiard3D
{
    internal static class TrackFactory
    {
        private const double Distance = 975.0;
        private const double Width = 640.0;
        private const double HeightHigher = 657.0;
        private const double HeightLower = 215.0;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Room RoomWithPlaneRoof(double radius)
        {
            var walls = CreateWalls();
            walls = CreateCylinders(walls.ToList(), radius);
            walls = CreateSpheres(walls.ToList(), radius);

            return new Room(walls, radius);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerable<ITrackBoundary> CreateWalls()
        {
            var firstRightBottom = (0D, 0d, 0d);
            var firstRightTop = (0D, 0d, HeightHigher);
            var firstLeftBottom = (0D, Width, 0d);
            var firstLeftTop = (0d, Width, HeightHigher);

            var secondRightBottom = (Distance, 0d, 0d);
            var secondRightTop = (Distance, 0d, HeightHigher);
            var secondLeftBottom = (Distance, Width, 0d);
            var secondLeftTop = (Distance, Width, HeightHigher);

            var frontWall = new Wall(new Vector3D[] {firstRightBottom, firstRightTop, firstLeftTop, firstLeftBottom})
            {
                NormalVector = (1, 0, 0),
                BoundaryName = 0.ToString()
            };
            var oppositeWall = new Wall(new Vector3D[]
                {secondRightBottom, secondRightTop, secondLeftTop, secondLeftBottom})
            {
                NormalVector = (-1, 0, 0),
                BoundaryName = 1.ToString()
            };
            var rightWall =
                new Wall(new Vector3D[] {firstRightBottom, secondRightBottom, secondRightTop, firstRightTop})
                {
                    NormalVector = (0, 1, 0),
                    BoundaryName = 2.ToString()
                };
            var leftWall = new Wall(new Vector3D[] {firstLeftBottom, secondLeftBottom, secondLeftTop, firstLeftTop})
            {
                NormalVector = (0, -1, 0),
                BoundaryName = 3.ToString()
            };
            var roof = new Wall(new Vector3D[] {firstRightTop, secondRightTop, secondLeftTop, firstLeftTop})
            {
                NormalVector = (0, 0, -1),
                BoundaryName = 4.ToString()
            };
            var floor = new Wall(
                new Vector3D[] {firstRightBottom, secondRightBottom, secondLeftBottom, firstLeftBottom})
            {
                NormalVector = (0, 0, 1),
                BoundaryName = 5.ToString()
            };

            return new List<Wall> {frontWall, oppositeWall, rightWall, leftWall, roof, floor};
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Room RoomWithTiltedRoof(double radius)
        {
            var firstRightBottom = (0D, 0d, 0d);
            var firstRightTop = (0D, 0d, HeightLower);
            var firstLeftBottom = (0D, Width, 0d);
            var firstLeftTop = (0d, Width, HeightLower);

            var secondRightBottom = (Distance, 0d, 0d);
            var secondRightTop = (Distance, 0d, HeightHigher);
            var secondLeftBottom = (Distance, Width, 0d);
            var secondLeftTop = (Distance, Width, HeightHigher);

            var frontWall = new Wall(new Vector3D[] {firstRightBottom, firstRightTop, firstLeftTop, firstLeftBottom});
            var oppositeWall = new Wall(new Vector3D[]
                {secondRightBottom, secondRightTop, secondLeftTop, secondLeftBottom});
            var rightWall =
                new Wall(new Vector3D[] {firstRightBottom, secondRightBottom, secondRightTop, firstRightTop});
            var leftWall = new Wall(new Vector3D[] {firstLeftBottom, secondLeftBottom, secondLeftTop, firstLeftTop});
            var roof = new Wall(new Vector3D[] {firstRightTop, secondRightTop, secondLeftTop, firstLeftTop});
            var floor = new Wall(
                new Vector3D[] {firstRightBottom, secondRightBottom, secondLeftBottom, firstLeftBottom});

            var walls = new ITrackBoundary[] {frontWall, oppositeWall, rightWall, leftWall, roof, floor}.ToList();
            walls = CreateCylinders(walls.ToList(), radius);
            walls = CreateSpheres(walls.ToList(), radius);

            return new Room(walls, radius);
        }

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int AddSphere(List<Sphere> spheres, int index, Sphere sphere)
        {
            if (spheres.Exists(x => x.Center == sphere.Center))
                return index;

            var name = "S" + index;
            sphere.BoundaryName = name;
            spheres.Add(sphere);
            index++;

            return index;
        }

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static Sphere CreateSphereBarrier(IEnumerable<ITrackBoundary> trackObjects, [NotNull] Sphere sphere)
        {
            if (sphere == null) throw new ArgumentNullException(nameof(sphere));

            var walls = trackObjects.OfType<Wall>().GroupBy(x =>
                    x.Corners.Select(y => Vector3D.AbsoluteValue(y - sphere.Center)).Min())
                .OrderBy(x => x.Key).SelectMany(x => x).Take(3).ToList();
            var corner = walls.Select(x => x.Corners).Aggregate((x, y) => x.Intersect(y).ToList()).Single();

            var intersectionPoints = walls.Select(sphere.GetInterSection).ToList();
            var barrier = new Plane(intersectionPoints[0], intersectionPoints[1], intersectionPoints[2]);
            var sign = barrier.DeterminePointPosition(corner);
            var checker = new PointChecker(barrier, sign);

            sphere.Checker = checker;
            return sphere;
        }

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static List<ITrackBoundary> CreateSpheres(List<ITrackBoundary> trackObjects, double radius)
        {
            var cylinders = trackObjects.OfType<Cylinder>();
            var spheres = new List<Sphere>(8);
            var index = 0;
            foreach (var cylinder in cylinders)
            {
                var sphereA = CreateSphereBarrier(trackObjects, new Sphere(cylinder.TopCenter, radius));
                var sphereB = CreateSphereBarrier(trackObjects, new Sphere(cylinder.BottomCenter, radius));

                index = AddSphere(spheres, index, sphereA);
                index = AddSphere(spheres, index, sphereB);
            }

            trackObjects.AddRange(spheres);
            return trackObjects;
        }

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static Cylinder CalculateCylinder(Line first, Line second, Line wallLine, double radius)
        {
            var angle = Vector3D.Angle(first.Direction, second.Direction);
            var distance = Math.Sin(90.0.ToRadian()) / Math.Sin(angle / 2) * radius;
            var minimumWallDistance = Math.Sqrt(Math.Pow(distance, 2) - Math.Pow(radius, 2));

            var referencePoint = wallLine.Contains(first.BasePoint) ? first.BasePoint : first.SecondPoint;
            var firstSign = first.BasePoint == referencePoint ? +1 : -1;
            var secondSign = second.BasePoint == referencePoint ? +1 : -1;

            var firstPoint = first.GetPointOnLine(referencePoint, firstSign * minimumWallDistance);
            var secondPoint = second.GetPointOnLine(referencePoint, secondSign * minimumWallDistance);

            var crossLine = new Line(firstPoint, secondPoint);
            var directLine = new Line(referencePoint, crossLine.ClosestPoint(referencePoint));
            var top = directLine.GetPointOnLine(distance);

            var difference = top - referencePoint;
            var otherPoint = referencePoint == wallLine.BasePoint ? wallLine.SecondPoint : wallLine.BasePoint;
            var bottom = otherPoint + difference;

            var basLine = new Line(top, bottom);
            var reverseBase = new Line(bottom, top);

            var correctTop = basLine.GetPointOnLine(radius);
            var correctBottom = reverseBase.GetPointOnLine(radius);

            var wallHeight = Vector3D.AbsoluteValue(wallLine.SecondPoint - wallLine.BasePoint);
            var sign = otherPoint == wallLine.BasePoint ? -1 : +1;
            var bottomPoint = firstPoint + sign * wallHeight * (wallLine.SecondPoint - wallLine.BasePoint).Normalize();

            var barrier = new Plane(firstPoint, secondPoint, bottomPoint);
            var checker = new PointChecker(barrier, barrier.DeterminePointPosition(referencePoint));

            return new Cylinder(correctTop, correctBottom, checker, radius);
        }

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static List<ITrackBoundary> CreateCylinders(List<ITrackBoundary> objects, double radius)
        {
            var cylinders = new List<Cylinder>(12);
            foreach (var track in objects)
            {
                var wall = track as Wall ?? throw new ArgumentNullException(nameof(objects));
                for (var index = 0; index < wall.WallLines.Count; index++)
                {
                    var wallLine = wall.WallLines[index];
                    var otherWall = objects.Except(new[] {wall}).Cast<Wall>().ToList().Single(x =>
                        x.WallLines.Exists(line =>
                            line.BasePoint == wallLine.BasePoint && line.SecondPoint == wallLine.SecondPoint
                            || line.BasePoint == wallLine.SecondPoint && line.SecondPoint == wallLine.BasePoint));

                    var topLine = wall.WallLines.Single(line =>
                        line.BasePoint == wallLine.BasePoint && line.SecondPoint != wallLine.SecondPoint
                        || line.BasePoint == wallLine.SecondPoint && line.SecondPoint != wallLine.BasePoint);

                    var otherTopLine = otherWall.WallLines
                        .Except(new[] {new Line(wallLine.SecondPoint, wallLine.BasePoint), wallLine}).Single(x =>
                            x.BasePoint == topLine.BasePoint || x.BasePoint == topLine.SecondPoint || x.SecondPoint == topLine.BasePoint ||
                            x.SecondPoint == topLine.SecondPoint);

                    var cylinder = CalculateCylinder(topLine, otherTopLine, wallLine, radius);
                    if (cylinders.Exists(x => x.Contains(cylinder.TopCenter) && x.Contains(cylinder.BottomCenter)))
                        continue;
                    var name = "C" + cylinders.Count;
                    cylinder.BoundaryName = name;

                    cylinders.Add(cylinder);
                }
            }

            objects.AddRange(cylinders);
            return objects;
        }
    }
}