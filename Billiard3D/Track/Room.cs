using System;
using System.Collections.Generic;
using System.Linq;
using Billiard3D.VectorMath;

namespace Billiard3D.Track
{
    internal class Room
    {
        public Room(IEnumerable<Wall> walls)
        {
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
            for (var i = 0; i < NumberOfIterations; ++i)
            {
                var order = Walls.OrderBy(x => x.NormalEquation(initialPoint));
                var points = Walls.Except(new[] {hittedWall}).Select(x => x.NormalEquation(initialPoint));
                hittedWall = order.First(x => (x.NormalEquation(initialPoint) > 0) && (x != hittedWall));
                initialPoint += hittedWall.NormalEquation(initialPoint) * initialVel;
                initialVel = hittedWall.AngleAfterHit(initialPoint, initialVel);
            }
        }
    }
}