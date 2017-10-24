using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Billiard3D.Math;

namespace Billiard3D.Track
{
    internal class Playground
    {
        public List<Vector3D> Corners { get; set; } = new List<Vector3D>(8);

        private Vector3D CheckHit(Vector3D start, Vector3D vel)
        {
            var current = start + vel;
            while(Corners.Any(x => (x - current).Any(y => y > 0)))
            {

            }
            return null;
        }
    }
}
