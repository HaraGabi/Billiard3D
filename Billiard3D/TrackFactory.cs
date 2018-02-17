using Billiard3D.Track;
using Billiard3D.VectorMath;

namespace Billiard3D
{
    internal static class TrackFactory
    {
        private const double Distance = 975.0;
        private const double Width = 640.0;
        private const double HeightHigher = 657.0;
        private const double HeightLower = 215.0;

        public static Room RoomWithPlaneRoof(double radius)
        {
            CreateWalls(out var frontWall, out var oppositeWall, out var rightWall, out var leftWall, out var roof,
                out var floor);

            return new Room(new[] {frontWall, oppositeWall, rightWall, leftWall, roof, floor}, radius);
        }


        public static void CreateWalls(out Wall frontWall, out Wall oppositeWall, out Wall rightWall, out Wall leftWall,
            out Wall roof, out Wall floor)
        {
            var firstRightBottom = (0D, 0d, 0d);
            var firstRightTop = (0D, 0d, HeightHigher);
            var firstLeftBottom = (0D, Width, 0d);
            var firstLeftTop = (0d, Width, HeightHigher);

            var secondRightBottom = (Distance, 0d, 0d);
            var secondRightTop = (Distance, 0d, HeightHigher);
            var secondLeftBottom = (Distance, Width, 0d);
            var secondLeftTop = (Distance, Width, HeightHigher);

            frontWall = new Wall(new Vector3D[] {firstRightBottom, firstRightTop, firstLeftTop, firstLeftBottom})
            {
                NormalVector = (1, 0, 0)
            };
            oppositeWall = new Wall(new Vector3D[] {secondRightBottom, secondRightTop, secondLeftTop, secondLeftBottom})
            {
                NormalVector = (-1, 0, 0)
            };
            rightWall = new Wall(new Vector3D[] {firstRightBottom, secondRightBottom, secondRightTop, firstRightTop})
            {
                NormalVector = (0, 1, 0)
            };
            leftWall = new Wall(new Vector3D[] {firstLeftBottom, secondLeftBottom, secondLeftTop, firstLeftTop})
            {
                NormalVector = (0, -1, 0)
            };
            roof = new Wall(new Vector3D[] {firstRightTop, secondRightTop, secondLeftTop, firstLeftTop})
            {
                NormalVector = (0, 0, -1)
            };
            floor = new Wall(new Vector3D[] {firstRightBottom, secondRightBottom, secondLeftBottom, firstLeftBottom})
            {
                NormalVector = (0, 0, 1)
            };
        }

        public static Room RoomWithTiltedRoof(double radius)
        {
            var firstRightBottom = (0D, 0d, 0d);
            var firstRightTop = (0D, 0d, HeightLower);
            var firstLeftBottom = (0D, Width, 0d);
            var firstLeftTop = (0d, Width, HeightLower);

            var secondRightBottom = (Distance, 0d, 0d);
            var secondRightTop = (Distance, 0d, HeightHigher);
            var secondLeftBottom = (Distance, Width, 0d);
            var secondLeftTop = (Distance, Width, HeightHigher);

            var frontWall = new Wall(new Vector3D[] {firstRightBottom, firstRightTop, firstLeftTop, firstLeftBottom});
            var oppositeWall = new Wall(new Vector3D[]
                {secondRightBottom, secondRightTop, secondLeftTop, secondLeftBottom});
            var rightWall =
                new Wall(new Vector3D[] {firstRightBottom, secondRightBottom, secondRightTop, firstRightTop});
            var leftWall = new Wall(new Vector3D[] {firstLeftBottom, secondLeftBottom, secondLeftTop, firstLeftTop});
            var roof = new Wall(new Vector3D[] {firstRightTop, secondRightTop, secondLeftTop, firstLeftTop});
            var floor = new Wall(
                new Vector3D[] {firstRightBottom, secondRightBottom, secondLeftBottom, firstLeftBottom});

            return new Room(new[] {frontWall, oppositeWall, rightWall, leftWall, roof, floor}, radius);
        }
    }
}