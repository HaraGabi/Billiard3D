using Billiard3D.Track;
using Billiard3D.VectorMath;

namespace Billiard3D
{
    internal static class TrackFactory
    {
        public static Room RoomWithPlaneRoof(double radius)
        {
            const double distance = 975.0;
            const double width = 640.0;
            const double height = 657.0;

            var firstRightBottom = (0D, 0d, 0d);
            var firstRightTop = (0D, 0d, height);
            var firstLeftBottom = (0D, width, 0d);
            var firstLeftTop = (0d, width, height);

            var secondRightBottom = (distance, 0d, 0d);
            var secondRightTop = (distance, 0d, height);
            var secondLeftBottom = (distance, width, 0d);
            var secondLeftTop = (distance, width, height);

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

        public static Room RoomWithTiltedRoof(double radius)
        {
            const double distance = 975.0;
            const double width = 640.0;
            const double height1 = 657.0;
            const double height2 = 215.0;

            var firstRightBottom = (0D, 0d, 0d);
            var firstRightTop = (0D, 0d, height2);
            var firstLeftBottom = (0D, width, 0d);
            var firstLeftTop = (0d, width, height2);

            var secondRightBottom = (distance, 0d, 0d);
            var secondRightTop = (distance, 0d, height1);
            var secondLeftBottom = (distance, width, 0d);
            var secondLeftTop = (distance, width, height1);

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