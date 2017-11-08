using System;
using System.Collections.Generic;
using System.Linq;
using Billiard3D.Math;

namespace Billiard3D.Track
{
    internal class Room
    {
        private readonly double _roomHeight;
        private readonly double _roomLenght;
        private readonly double _roomWidth;

        public IEnumerable<Wall> Walls { get; set; }

        public int NumberOfIterations { get; set; } = 10_000;

        public Room(IEnumerable<Wall> walls)
        {
            Walls = walls;
        }

        public void StartSimulation(Vector3D initialPoint, Vector3D initialVel)
        {
            Vector3D currentPoint = initialPoint;
            var currentVel = initialVel;
            for (int i = 0; i < NumberOfIterations; ++i)
            {
                currentPoint += currentVel;

            }
        }
    }
}