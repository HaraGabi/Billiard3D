using System;
using System.Linq;
using JetBrains.Annotations;
using MathNet.Numerics.LinearAlgebra;

namespace Billiard3D.VectorMath
{
    internal struct Plane
    {
        public Vector3D NormalVector { get; }
        public Vector3D[] BasePoints { get; }

        public Plane([NotNull] Vector3D pointA, [NotNull] Vector3D pointB, [NotNull] Vector3D pointC)
        {
            if (pointA == null) throw new ArgumentNullException(nameof(pointA));
            if (pointB == null) throw new ArgumentNullException(nameof(pointB));
            if (pointC == null) throw new ArgumentNullException(nameof(pointC));

            NormalVector = Vector3D.CrossProduct(pointB - pointA, pointC - pointA).Normalize();
            BasePoints = new[] {pointA, pointB, pointC};
        }

        public double DeterminePointPosition([NotNull] Vector3D point)
        {
            if (point == null) throw new ArgumentNullException(nameof(point));

            var tildePoints = new[]
                {BasePoints[1] - BasePoints[0], BasePoints[2] - BasePoints[0], point - BasePoints[0]};

            var builder = Matrix<double>.Build;
            var matrix = builder.DenseOfColumnArrays(tildePoints.Select(x => x.ToArray()));
            return matrix.Determinant();
        }
    }
}