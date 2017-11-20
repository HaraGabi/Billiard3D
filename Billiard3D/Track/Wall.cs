using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Billiard3D.VectorMath;
using static System.Math;
using static Billiard3D.VectorMath.Vector3D;

namespace Billiard3D.Track
{
    [DebuggerDisplay("({NormalVector.X}, {NormalVector.Y}, {NormalVector.Z})")]
    internal class Wall
    {
        public Wall(IEnumerable<Vector3D> corners)
        {
            Corners.AddRange(corners);
            NormalVector = CrossProduct(Corners[1] - Corners[0], Corners[2] - Corners[0]).Normalize();
            if (!CheckIfPointIsOnThePlain(Corners.Last()))
                throw new ArgumentException();
        }

        public List<Vector3D> Corners { get; set; } = new List<Vector3D>();
        public Vector3D NormalVector { get; set; }
        public List<Vector3D> HittedPoints { get; set; } = new List<Vector3D>();

        private bool CheckIfPointIsOnThePlain((double x, double y, double z) point)
        {
            const double confidence = 0.000001;
            var forX = NormalVector.X * (point.x - Corners.First().X);
            var forY = NormalVector.Y * (point.y - Corners.First().Y);
            var forZ = NormalVector.Z * (point.z - Corners.First().Z);
            return forX + forY + forZ <= confidence;
        }

        public double NormalEquation(Vector3D tuple)
        {
            var ax = NormalVector.X * tuple.X;
            var by = NormalVector.Y * tuple.Y;
            var cz = NormalVector.Z * tuple.Z;
            var d = NormalVector.X * Corners.First().X + NormalVector.Y * Corners.First().Y +
                    NormalVector.Z * Corners.First().Z;
            return ax + by + cz - d;
        }

        public bool WasHit(Vector3D hitPoint) => CheckIfPointIsOnThePlain(hitPoint);

        public Vector3D AngleAfterHit(Vector3D hitPoint, Vector3D velocity)
        {
            var normalVel = velocity.Normalize();
            //if (!WasHit(hitPoint))
            //throw new ArgumentException("Collision not detected!");
            HittedPoints.Add(hitPoint);
            var ret = 2 * (-1 * normalVel * NormalVector) * NormalVector + normalVel;
            var first = Angle(velocity, NormalVector);
            first = first > PI / 2 ? Abs(first - PI) : first;
            var second = Angle(ret, NormalVector);
            second = second > PI / 2 ? Abs(second - PI) : second;
            if (Sin(first) - Sin(second) > 0.0005)
                throw new InvalidOperationException($"incoming {first.ToDegree()} going out {second.ToDegree()}");
            return ret;
        }
    }
}