using System.Linq;
using System.Runtime.CompilerServices;
using MathNet.Numerics.LinearAlgebra;

namespace Billiard3D.VectorMath
{
    internal readonly struct Plane
    {
        public Vector3D NormalVector { get; }
        private Vector3D[] BasePoints { get; }

        public Plane(Vector3D pointA, Vector3D pointB, Vector3D pointC)
        {
            NormalVector = Vector3D.CrossProduct(pointB - pointA, pointC - pointA).Normalize();
            BasePoints = new[] {pointA, pointB, pointC};
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public double DeterminePointPosition(Vector3D point)
        {
            var tildePoints = new[]
                {BasePoints[1] - BasePoints[0], BasePoints[2] - BasePoints[0], point - BasePoints[0]};

            var builder = Matrix<double>.Build;
            var matrix = builder.DenseOfColumnArrays(tildePoints.Select(x => x.ToArray()));
            return matrix.Determinant();
        }
    }
}