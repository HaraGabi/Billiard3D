using System.Linq;
using System.Runtime.CompilerServices;
using Billiard3D.VectorMath;

namespace Billiard3D.Track
{
    internal class CausticSphere
    {
        // ReSharper disable once MemberCanBePrivate.Global
        public Sphere Sphere { get; }

        public CausticSphere()
        {
            var sphere = new Sphere((50, 50, 50), 50);
            var plane = new Plane((50, 50, 0), (50, 0, 50), (0, 50, 50));
            var sign = plane.DeterminePointPosition((0, 0, 0)) > 0 ? +1 : -1;
            var checker = new PointChecker(plane, sign);
            sphere.Checker = checker;
            Sphere = sphere;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Line Start(Line startPoint)
        {
            var intersectionPoints = Sphere.GetIntersectionPoints(in startPoint);
            var intersect = intersectionPoints.Single();
            var line = Sphere.LineAfterHit(in startPoint, in intersect);
            return line;
        }
    }

    internal class CausticSphere2
    {
        // ReSharper disable once MemberCanBePrivate.Global
        public Sphere Sphere { get; }

        public CausticSphere2()
        {
            var sphere = new Sphere((0, 0, 0), 1);
            var plane = new Plane((2, 3, 1), (4, 3, 1), (0, 0, 1));
            var sign = plane.DeterminePointPosition((0, 0, 0)) > 0 ? +1 : -1;
            var checker = new PointChecker(plane, sign);
            sphere.Checker = checker;
            Sphere = sphere;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Line Start(Line startPoint)
        {
            var intersectionPoints = Sphere.GetIntersectionPoints(in startPoint);
            var intersect = intersectionPoints.Single();
            var line = Sphere.LineAfterHit(in startPoint, in intersect);
            return line;
        }
    }
}