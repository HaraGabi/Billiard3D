using Billiard3D.VectorMath;
using JetBrains.Annotations;
using System.Runtime.CompilerServices;

namespace Billiard3D
{
    internal struct PointChecker : IPointChecker
   {
        private Plane Plane { get; }
        private bool IsPositive { get; }

        public PointChecker(Plane plane, double sign)
        {
            IsPositive = sign > 0;
            Plane = plane;
        }

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsPointOnTheCorrectSide(Vector3D point)
        {
            var determinant = Plane.DeterminePointPosition(point);

            return determinant < 0 && !IsPositive || determinant > 0 && IsPositive;
        }
    }
}