using Billiard3D.Track;
using Billiard3D.VectorMath;

namespace Billiard3D
{
    internal class SphereChecker : IPointChecker
    {
        public Vector3D PointA { get; }
        public Vector3D PointB { get; }
        public Vector3D PointC { get; }
        public Vector3D BasePoint { get; }
        public Vector3D Center { get; }
        public PointChecker TopChecker { get; }
        public PointChecker SideCheckerA { get; }
        public PointChecker SideCheckerB { get; }

        public SphereChecker(Vector3D pointA, Vector3D pointB, Vector3D pointC, Vector3D basePoint, Vector3D center)
        {
            PointA = pointA;
            PointB = pointB;
            PointC = pointC;
            BasePoint = basePoint;
            Center = center;
            TopChecker = GetTopChecker();
            SideCheckerA = GetSideChecker(PointA);
            SideCheckerB = GetSideChecker(PointB);
        }

        public bool IsPointOnTheCorrectSide(Vector3D point) =>
            TopChecker.IsPointOnTheCorrectSide(point) && SideCheckerA.IsPointOnTheCorrectSide(point) &&
            SideCheckerB.IsPointOnTheCorrectSide(point);

        private PointChecker GetSideChecker(Vector3D point)
        {
            var sideAPlane = new Plane(point, Center, PointC);
            var signA = sideAPlane.DeterminePointPosition(BasePoint);
            return new PointChecker(sideAPlane, signA);
        }

        private PointChecker GetTopChecker()
        {
            var topPlane = new Plane(PointA, PointB, Center);
            var sign = topPlane.DeterminePointPosition(BasePoint);
            return new PointChecker(topPlane, sign);
        }
    }
}