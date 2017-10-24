using Billiard3D.Math;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Billiard3D.Track
{
    internal class Room
    {
        private double _roomLenght;
        private double _roomWidth;
        private double _roomHeight;

        public List<Vector3D> Corners { get; set; } = new List<Vector3D>();

        public Room(List<Vector3D> corners)
        {
            if (corners.Count != 8)
            {
                throw new ArgumentException($"Not enough points to constructs a room {nameof(corners)}");
            }
            Corners.AddRange(corners);
            _roomLenght = Corners.Max(x => x.X) - Corners.Min(x => x.X);
            _roomWidth = Corners.Max(x => x.Y) - Corners.Min(x => x.Y);
            _roomHeight = Corners.Max(x => x.Z) - Corners.Min(x => x.Z);
        }

        public void StartSimulation(Vector3D startingPoint, Vector3D startingDirection)
        {
            var actualPosition = startingDirection + startingDirection;
            while(InRoom(actualPosition))
            {
                actualPosition += startingDirection;
            }
        }

        private bool InRoom(Vector3D vector)
        {
            if (vector.X < _roomLenght && vector.Y < _roomWidth && vector.Z < _roomHeight)
            {
                return true;
            }
            return false;
        }
    }
}
