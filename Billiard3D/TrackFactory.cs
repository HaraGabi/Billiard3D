using Billiard3D.Track;
using Billiard3D.VectorMath;

namespace Billiard3D
{
    internal static class TrackFactory
    {
        public static Room RoomWithPlaneRoof(double radius)
        {
            CreateWalls(out var frontWall, out var oppositeWall, out var rightWall, out var leftWall, out var roof, out var floor);

            return new Room(new[] { frontWall, oppositeWall, rightWall, leftWall, roof, floor }, radius);
        }

        public static PureRoom CreatePurePlaneRoof()
        {
            CreateWalls(out var frontWall, out var oppositeWall, out var rightWall, out var leftWall, out var roof, out var floor);
            return new PureRoom(new[] { frontWall, oppositeWall, rightWall, leftWall, roof, floor });
        }

        public static void CreateWalls(out Wall frontWall, out Wall oppositeWall, out Wall rightWall, out Wall leftWall, out Wall roof, out Wall floor)
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

            frontWall = new Wall(new Vector3D[] { firstRightBottom, firstRightTop, firstLeftTop, firstLeftBottom })
            {
                NormalVector = (1, 0, 0)
            };
            oppositeWall = new Wall(new Vector3D[] { secondRightBottom, secondRightTop, secondLeftTop, secondLeftBottom })
            {
                NormalVector = (-1, 0, 0)
            };
            rightWall = new Wall(new Vector3D[] { firstRightBottom, secondRightBottom, secondRightTop, firstRightTop })
            {
                NormalVector = (0, 1, 0)
            };
            leftWall = new Wall(new Vector3D[] { firstLeftBottom, secondLeftBottom, secondLeftTop, firstLeftTop })
            {
                NormalVector = (0, -1, 0)
            };
            roof = new Wall(new Vector3D[] { firstRightTop, secondRightTop, secondLeftTop, firstLeftTop })
            {
                NormalVector = (0, 0, -1)
            };
            floor = new Wall(new Vector3D[] { firstRightBottom, secondRightBottom, secondLeftBottom, firstLeftBottom })
            {
                NormalVector = (0, 0, 1)
            };
        }

        public static TiltedRoom RoomWithTiltedRoof(double radius)
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

            var frontWall =
                new Wall(new Vector3D[] {firstRightBottom, firstRightTop, firstLeftTop, firstLeftBottom})
                {
                    NormalVector = (1, 0, 0)
                };
            var oppositeWall = new Wall(new Vector3D[]
                {secondRightBottom, secondRightTop, secondLeftTop, secondLeftBottom}) {NormalVector = (-1, 0, 0)};
            var rightWall =
                new Wall(new Vector3D[] {firstRightBottom, secondRightBottom, secondRightTop, firstRightTop})
                {
                    NormalVector = (0, 1, 0)
                };
            var leftWall =
                new Wall(new Vector3D[] {firstLeftBottom, secondLeftBottom, secondLeftTop, firstLeftTop})
                {
                    NormalVector = (0, -1, 0)
                };
            var roof = new Wall(new Vector3D[] {firstRightTop, secondRightTop, secondLeftTop, firstLeftTop});
            roof.NormalVector = (- 1 * roof.NormalVector.X, roof.NormalVector.Y, -1 * roof.NormalVector.Z);
            var floor = new Wall(
                    new Vector3D[] {firstRightBottom, secondRightBottom, secondLeftBottom, firstLeftBottom})
                {NormalVector = (0, 0, 1)};

            return new TiltedRoom(new[] {frontWall, oppositeWall, rightWall, leftWall, roof, floor}, radius);
        }
    }
}