using System;
using System.Collections.Generic;
using System.Linq;
using Billiard3D.Math;

namespace Billiard3D.Track
{
    internal class Room
    {
        public IEnumerable<Wall> Walls { get; set; }

        public int NumberOfIterations { get; set; } = 10_000;

        public Room(IEnumerable<Wall> walls)
        {
            Walls = walls;
        }

        public void StartSimulation(Vector3D initialPoint, Vector3D initialVel)
        {
            for (int i = 0; i < NumberOfIterations; ++i)
            {
                var hittedWall = Walls.OrderBy(x => x.NormalEquation(initialPoint)).First();
                initialPoint += System.Math.Abs(hittedWall.NormalEquation(initialPoint)) * initialVel;
                initialVel = hittedWall.AngleAfterHit(initialPoint, initialVel);
            }
        }
    }
}