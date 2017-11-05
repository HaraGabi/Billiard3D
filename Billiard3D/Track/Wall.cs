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
        public Vector3D NormalVector { get; set; }

        public Wall(ICollection<Vector3D> corners)
        {
            Corners.AddRange(corners);
            NormalVector = Vector3D.Vectorial(Corners[1] - Corners[0], Corners[2]-Corners[0]);
            if (!CheckIfPointIsOnThePlain(Corners.Last()))
            {
                throw new ArgumentException();
            }
        }

        private bool CheckIfPointIsOnThePlain((double x, double y, double z) point)
        {
            double confidende = 0.000001;
            double forX = NormalVector.X * (point.x - Corners.First().X);
            double forY = NormalVector.Y * (point.y - Corners.First().Y);
            double forZ = NormalVector.Z * (point.z - Corners.First().Z);
            return forX + forY + forZ <= confidende;
        }

        public double NormalEquation(Vector3D tuple) => NormalVector * (Corners.First() - tuple);

        public bool WasHit(Vector3D hitPoint) => CheckIfPointIsOnThePlain(hitPoint);

        public Vector3D AngleAfterHit(Vector3D hitPoint)
        {
            if (!WasHit(hitPoint))
                throw new ArgumentException("Collision not detected!");
            return null;
        }
    }
}
