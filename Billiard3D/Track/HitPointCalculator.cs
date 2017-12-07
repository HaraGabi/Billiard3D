using Billiard3D.VectorMath;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Billiard3D.Track
{
    internal class HitPointCalculator
    {
        public double CoveredArea { get; }

        public HitPointCalculator(double coveredArea)
        {
            CoveredArea = coveredArea;
        }

        public Vector3D CalculateCorrectHitPoint(ITrackObject trackObject, Line currentLine)
        {
            var hitpoints = trackObject.GetIntersectionPoints(currentLine);
            var points = hitpoints.Item1;
            var possibleHitPoints = points.Where(x => x.Item2 > 0);
        }
    }
}
