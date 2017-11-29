using System;
using System.Collections.Generic;
using System.Linq;
using Billiard3D.VectorMath;

namespace Billiard3D.Track
{
    internal class Room
    {
        public double Radius { get; }

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
                currentPoint += hittedWall.NormalEquation(initialPoint) * initialVel;
                initialVel = hittedWall.AngleAfterHit(initialPoint, initialVel);
            }
        }

        private void SetWallBorderArea(Wall hitted, Vector3D hittedPoint)
        {
            var otherCornersInOrderOfDistance = hitted.Corners.OrderBy(x => Vector3D.AbsoluteValue(x - hittedPoint)).ToList();
            var closestCorner = otherCornersInOrderOfDistance.First();
            var otherWall = Walls.Except(new[] {hitted}).Where(x => x.Corners.Any(corner => corner == closestCorner))
                .OrderBy(x => x.Corners.Min(y => Vector3D.AbsoluteValue(y - hittedPoint))).First();
            var angle = Vector3D.Angle(otherWall.NormalVector, hitted.NormalVector);
            var maximumDistance = CalculateDistance(angle);
            var distanceOnWall = Math.Sqrt(Math.Pow(maximumDistance, 2) - Math.Pow(Radius, 2));

            var secondClosestCorner = otherCornersInOrderOfDistance.Skip(1).First();
        }

        private double CalculateDistance(double angle) => Math.Sin(angle / 2) / Math.Sin(45.0.ToRadian()) * (1 / Radius);
    }
}