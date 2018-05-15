using System.Linq;
using System.Runtime.CompilerServices;
using Billiard3D.VectorMath;
using static System.Math;

namespace Billiard3D.Track
{
    internal class CausticCylinder
    {
        public static double R { get; set; } = 1;
        public static double L { get; set; } = 100;
        public Cylinder Cylinder { get; }

        public CausticCylinder()
        {
            var plane = new Plane((0, 0, 0), (0, L, 0), (0, L, L));
            var sign = plane.DeterminePointPosition((-1, 0, 0)) > 0 ? +1 : -1;
            var pcs = new PointChecker(plane, sign);
            Cylinder = new Cylinder((0, 0, 0), (0, L, 0), pcs, R);
        }

        public CausticCylinder(double quarter)
        {
            var plane = new Plane((0, 0, -R), (-R, 0, 0), (0, L, -R));
            var sign = plane.DeterminePointPosition((-1, 0, -1)) > 0 ? +1 : -1;
            var checker = new PointChecker(plane, sign);
            Cylinder = new Cylinder((0, 0, 0), (0, L, 0), checker, R);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Line Start(Line startPoint)
        {
            var intersectionPoints = Cylinder.GetIntersectionPoints(in startPoint).ToList();
            if (intersectionPoints.Count > 1) return new Line((-90, -90, -90), (-99, -99, -99));
            if (intersectionPoints.Count == 0) return new Line((-90, -90, -90), (-99, -99, -99));
            var intersect = intersectionPoints.Single();
            var line = Cylinder.LineAfterHit(in startPoint, in intersect);
            return line;
        }
    }
}