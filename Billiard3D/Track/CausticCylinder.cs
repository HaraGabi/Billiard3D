using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Billiard3D.VectorMath;

namespace Billiard3D.Track
{
    internal class CausticCylinder
    {
        public Cylinder Cylinder { get; }
        public CausticCylinder()
        {
            const double r = 25;
            var negyed = r / 4;
            var plane = new Plane((negyed, 0, 0), (0, 0, negyed), (negyed, 100, 0));
            var pcs = new PointChecker(plane, -1);
            Cylinder = new Cylinder((0, 0, 0), (0, 100, 0), pcs, r);
        }

        public Line Start(Line startPoint)
        {
            var intersectionPoints = Cylinder.GetIntersectionPoints(in startPoint);
            var intersect = intersectionPoints.Single();
            var line = Cylinder.LineAfterHit(in startPoint, in intersect);
            return line;
        }
    }
}
