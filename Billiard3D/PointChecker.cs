﻿using Billiard3D.VectorMath;
using JetBrains.Annotations;

namespace Billiard3D
{
    internal struct PointChecker
    {
        public PointChecker(Plane plane, double sign)
        {
            IsPositive = sign > 0;
            Plane = plane;
        }

        private Plane Plane { get; }
        private bool IsPositive { get; }

        [Pure]
        public bool IsPointOnTheCorrectSide(Vector3D point)
        {
            var determinant = Plane.DeterminePointPosition(point);

            return ((determinant < 0) && !IsPositive) || ((determinant > 0) && IsPositive);
        }
    }
}