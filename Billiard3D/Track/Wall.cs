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
            NormalVector = Vector3D.Vektorial(Corners[1] - Corners[0], Corners[2]-Corners[0]);
            if (!Equation(Corners.Last()))
            {
                throw new ArgumentException();
            }
        }

        private bool Equation((double x, double y, double z) tup)
        {
            double confidende = 0.000001;
            return NormalVector.X * (tup.x - Corners[0].X) + NormalVector.Y * (tup.y - Corners[0].Y) + NormalVector.Z * (tup.z - Corners[0].Z) >= confidende;
        }

        public double NormalEquation(Vector3D tuple) => NormalVector * (Corners.First() - tuple);

        public bool WasHit(Vector3D hitPoint) => Equation(hitPoint);

        public Vector3D AngleAfterHit(Vector3D hitPoint)
        {
            if (!WasHit(hitPoint))
                throw new ArgumentException("Collision not detected!");
            var list = new List<double>();
            Corners.ForEach(x => list.Add(Vector3D.Angle(x, hitPoint))); // nope not working
            throw new NotImplementedException();
        }
    }
}
