using System;
using Billiard3D.VectorMath;
using JetBrains.Annotations;

namespace Billiard3D.Track
{
    internal class Corner
    {
        public Corner([NotNull] Vector3D beginPoint, [NotNull] Vector3D endPoint, double radius)
        {
            BeginPoint = beginPoint ?? throw new ArgumentNullException(nameof(beginPoint));
            EndPoint = endPoint ?? throw new ArgumentNullException(nameof(endPoint));
            CircleRadius = radius;
        }

        [NotNull]
        public Vector3D BeginPoint { get; }

        [NotNull]
        public Vector3D EndPoint { get; }

        public double CircleRadius { get; }

    }
}