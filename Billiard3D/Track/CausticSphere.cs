using System;
using System.Linq;
using System.Runtime.CompilerServices;
using Billiard3D.VectorMath;

namespace Billiard3D.Track
{
    internal class CausticSphere
    {
        public static double R { get; set; } = 1;
        // ReSharper disable once MemberCanBePrivate.Global
        public Sphere Sphere { get; }

        public CausticSphere()
        {
            var sphere = new Sphere((0, 0, 1), R);
            var plane = new Plane((2, 3, 1), (4, 3, 1), (0, 0, 1));
            var sign = plane.DeterminePointPosition((0, 0, 0)) > 0 ? +1 : -1;
            var checker = new PointChecker(plane, sign);
            sphere.Checker = checker;
            Sphere = sphere;
        }

        public CausticSphere(double octave)
        {
            var sphere = new Sphere((0, 0, 1), R);
            var position = 1 / octave;
            var plane = new Plane((position, 0, 1 + position), (0, position, 1 + position), (position, position, 0));
            var sign = plane.DeterminePointPosition((0, 0, 0)) > 0 ? +1 : -1;
            var checker = new PointChecker(plane, sign);
            sphere.Checker = checker;
            Sphere = sphere;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Line Start(Line startPoint)
        {
            var intersectionPoints = Sphere.GetIntersectionPoints(in startPoint).ToList();
            if (intersectionPoints.Count > 1) throw new InvalidOperationException();
            var intersect = intersectionPoints.Single();
            var line = Sphere.LineAfterHit(in startPoint, in intersect);
            return line;
        }
    }
}