using System;
using System.Linq;
using Billiard3D.Track;
using JetBrains.Annotations;

namespace Billiard3D.VectorMath
{
    internal static class SphereWallInterSector
    {
        public static Vector3D GetInterSection([NotNull] this Sphere sphere, [NotNull] Wall wall)
        {
            if (sphere is null) throw new ArgumentNullException(nameof(sphere));
            if (wall is null) throw new ArgumentNullException(nameof(wall));

            (var a, var b, var c) = wall.NormalVector;
            var (x0, y0, z0) = wall.Corners.First();
            var (xs, ys, zs) = sphere.Center;

            var d = -(a * x0 + b * y0 + c * z0);

            var squareResult = a * a + b * b + c * c;
            var equation = a * xs + b * ys + c * zs + d;
            var sameInEvery = equation / squareResult;

            var xc = xs - a * sameInEvery;
            var yc = ys - b * sameInEvery;
            var zc = zs - c * sameInEvery;

            return (xc, yc, zc);
        }
    }
}