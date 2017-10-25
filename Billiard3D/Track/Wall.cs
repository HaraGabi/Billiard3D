using System;
using System.Collections.Generic;
using System.Text;
using Billiard3D.Math;
using System.Linq;

namespace Billiard3D.Track
{
    internal class Wall
    {
        public List<Vector3D> Corners { get; set; } = new List<Vector3D>();

        public Wall(ICollection<Vector3D> corners)
        {
            Corners.AddRange(corners);
        }

        public bool WasHit(Vector3D hitPoint)
        {
            var (x, y, z) = hitPoint;
            var goodForX = x >= Corners.Min(vector => vector.X) && x <= Corners.Max(vector => vector.X);
            var goodForY = y >= Corners.Min(vector => vector.Y) && y <= Corners.Max(vector => vector.Y);
            var goodForZ = z >= Corners.Min(vector => vector.Z) && z <= Corners.Max(vector => vector.Z);
            return goodForX && goodForY && goodForZ;
        }
    }
}
