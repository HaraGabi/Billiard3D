using System;
using System.Collections.Generic;
using System.Text;
using Billiard3D.VectorMath;
using JetBrains.Annotations;

namespace Billiard3D.Track
{
    internal interface ITrackObject
    {
        IEnumerable<Vector3D> GetIntersectionPoints([NotNull] Line line);

        Line LineAfterHit([NotNull] Line incoming, [NotNull] Vector3D hittedPoint);
    }
}
