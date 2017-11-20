using System;
using Billiard3D.VectorMath;
using JetBrains.Annotations;

namespace Billiard3D.Track
{
    internal class Corner
    {
        public Corner([NotNull] Vector3D cornerPoint) =>
            CornerPoint = cornerPoint ?? throw new ArgumentNullException(nameof(cornerPoint));

        public Corner([NotNull] Vector3D cornerPoint, double r)
            : this(cornerPoint)
        {
        }

        [NotNull]
        public Vector3D CornerPoint { get; }
    }
}