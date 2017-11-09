using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using Billiard3D.Math;

namespace Billiard3D.Track
{
    internal class Room
    {
        public Room(IEnumerable<Wall> walls) => Walls = walls;

        public IEnumerable<Wall> Walls { get; set; }

        public int NumberOfIterations { get; set; } = 10_000;

        public void StartSimulation(Vector3D initialPoint, Vector3D initialVel)
        {
            for (var i = 0; i < NumberOfIterations; ++i)
            {
                var order = Walls.OrderBy(x => x.NormalEquation(initialPoint));
                var hittedWall = order.First(x => x.NormalEquation(initialPoint) > 0);
                initialPoint += System.Math.Abs(hittedWall.NormalEquation(initialPoint)) * initialVel;
                initialVel = hittedWall.AngleAfterHit(initialPoint, initialVel);
            }
        }
    }
}