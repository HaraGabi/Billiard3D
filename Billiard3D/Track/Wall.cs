using System;
using System.Collections.Generic;
using System.Text;
using Billiard3D.Math;
using System.Linq;
using static System.Math;
using static Billiard3D.Math.Vector3D;

namespace Billiard3D.Track
{
    internal class Wall
    {
        public List<Vector3D> Corners { get; set; } = new List<Vector3D>();
        public Vector3D NormalVector { get; set; }
        public List<Vector3D> HittedPoints { get; set; } = new List<Vector3D>();

        public Wall(ICollection<Vector3D> corners)
        {
            Corners.AddRange(corners);
            NormalVector = Vectorial(Corners[1] - Corners[0], Corners[2]-Corners[0]).Normalize();
            if (!CheckIfPointIsOnThePlain(Corners.Last()))
                throw new ArgumentException();
        }

        private bool CheckIfPointIsOnThePlain((double x, double y, double z) point)
        {
            const double confidende = 0.000001;
            double forX = NormalVector.X * (point.x - Corners.First().X);
            double forY = NormalVector.Y * (point.y - Corners.First().Y);
            double forZ = NormalVector.Z * (point.z - Corners.First().Z);
            return forX + forY + forZ <= confidende;
        }

        public double NormalEquation(Vector3D tuple)
        {
            var vec = (Corners.First() - tuple);
            var val = NormalVector * vec;
            return val;
        }

        public bool WasHit(Vector3D hitPoint) => CheckIfPointIsOnThePlain(hitPoint);

        public Vector3D AngleAfterHit(Vector3D hitPoint, Vector3D velocity)
        {
            var normalVel = velocity.Normalize();
            //if (!WasHit(hitPoint))
                //throw new ArgumentException("Collision not detected!");
            HittedPoints.Add(hitPoint);
            Vector3D ret = 2 * ((-1 * normalVel) * NormalVector) * NormalVector + normalVel;
            double first = Angle(velocity, NormalVector);
            first = first > PI / 2 ? Abs(first - PI) : first; 
            double second = Angle(ret, NormalVector);
            second = second > PI / 2 ? Abs(second - PI) : second;
            if (Sin(first) - Sin(second) > 0.0005)
                throw new InvalidOperationException($"incoming {first.ToDegree()} going out {second.ToDegree()}");
            return ret;
        }
    }
}
