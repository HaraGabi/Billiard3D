using System;
using System.Collections.Generic;
using System.Linq;
using Billiard3D.VectorMath;
using JetBrains.Annotations;

namespace Billiard3D.Track
{
    internal class Room
    {
        public Room(IEnumerable<Wall> walls, double radius = 0.003)
        {
            Radius = radius;
            Walls = new List<Wall>();
            foreach (var wall in walls)
            {
                if (Walls.Any(x => x.NormalVector == wall.NormalVector))
                    wall.NormalVector *= -1;
                Walls.Add(wall);
            }
        }

        public double Radius { get; }

        public List<Wall> Walls { get; set; }

        public int NumberOfIterations { get; set; } = 10_000;

        public void StartSimulation(Vector3D initialPoint, Vector3D initialVel)
        {
            var hittedWall = Walls.First(x => Math.Abs(x.NormalEquation(initialPoint)) < 0.00005);
            var currentPoint = initialPoint;
            for (var i = 0; i < NumberOfIterations; ++i)
            {
                var order = Walls.OrderBy(x => x.NormalEquation(currentPoint));
                var points = Walls.Except(new[] {hittedWall}).Select(x => x.NormalEquation(currentPoint));
                hittedWall = order.First(x => (x.NormalEquation(initialPoint) > 0) && (x != hittedWall));
                var expectedHitPoint = hittedWall.NormalEquation(initialPoint) * initialVel;
                var isCorner = WasCornerHit(hittedWall, expectedHitPoint);
                if (isCorner)
                {
                    
                }
                currentPoint += hittedWall.NormalEquation(initialPoint) * initialVel;
                initialVel = hittedWall.AngleAfterHit(initialPoint, initialVel);
            }
        }

        private bool WasCornerHit(Wall hitted, Vector3D hittedPoint)
        {
            var closestCorner = GetNthClosestCorner(hittedPoint, hitted, 0);
            var otherWall = Walls.Except(new[] {hitted}).Where(x => x.Corners.Any(corner => corner == closestCorner))
                .OrderBy(x => x.Corners.Min(y => Vector3D.AbsoluteValue(y - hittedPoint))).First();
            var angle = Vector3D.Angle(otherWall.NormalVector, hitted.NormalVector);
            var maximumDistance = CalculateDistance(angle);
            var distanceOnWall = Math.Sqrt(Math.Pow(maximumDistance, 2) - Math.Pow(Radius, 2));

            var secondClosestCorner = GetNthClosestCorner(hittedPoint, hitted, 1);
            var line = new Line(closestCorner, secondClosestCorner);
            return distanceOnWall <= line.DistanceFrom(hittedPoint);
        }

        private Vector3D GetNthClosestCorner([NotNull] Vector3D referencePoint, [NotNull] Wall hittedWall, int which)
        {
            if (referencePoint == null) throw new ArgumentNullException(nameof(referencePoint));
            if (hittedWall == null) throw new ArgumentNullException(nameof(hittedWall));
            if ((which < 0) || (which > 3)) throw new ArgumentException(nameof(which));
            return hittedWall.Corners.OrderBy(x => Vector3D.AbsoluteValue(x - referencePoint)).Skip(which).First();
        }

        /// <summary>
        ///     Calculates the distance where the center of the sphere should be using the sine theorem
        /// </summary>
        /// <param name="angle">The angle.</param>
        /// <returns></returns>
        private double CalculateDistance(double angle) =>
            Math.Sin(angle / 2) / Math.Sin(45.0.ToRadian()) * (1 / Radius);
    }
}