﻿using System.Collections.Generic;
using Billiard3D.VectorMath;
using JetBrains.Annotations;

namespace Billiard3D.Track
{
    internal interface ITrackObject
    {
        string ObjectName { get; set; }

        List<Vector3D> HitPoints { get; }
        (IEnumerable<(Vector3D, double)>, ITrackObject) GetIntersectionPoints([NotNull] Line line);

        Line LineAfterHit([NotNull] Line incoming, [NotNull] Vector3D hitPoint);
    }
}