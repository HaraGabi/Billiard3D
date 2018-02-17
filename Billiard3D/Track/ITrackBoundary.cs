using System.Collections.Generic;
using Billiard3D.VectorMath;

namespace Billiard3D.Track
{
    internal interface ITrackBoundary
    {
        string BoundaryName { get; }

        List<Vector3D> HitPoints { get; }
        IEnumerable<Vector3D> GetIntersectionPoints(in Line line);

        Line LineAfterHit(in Line incoming, in Vector3D hitPoint);
        bool IsInCorrectPosition(Line ball);
    }
}