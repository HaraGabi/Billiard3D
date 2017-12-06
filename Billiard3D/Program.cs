using System;
using System.Linq;
using Billiard3D.Track;
using Billiard3D.VectorMath;
using JetBrains.Annotations;

namespace Billiard3D
{
    [UsedImplicitly]
    public class Programs
    {
        public static void Main(string[] args)
        {
            var rightWall = new Wall(new Vector3D[] {(0, 0, 0), (300, 0, 0), (300, 0, 300), (0, 0, 300)});
            var leftFal = new Wall(new Vector3D[] {(0, 300, 0), (300, 300, 0), (300, 300, 300), (0, 300, 300)});
            var backWall = new Wall(new Vector3D[] {(0, 0, 0), (0, 300, 0), (0, 300, 300), (0, 0, 300)});
            var frontWall = new Wall(new Vector3D[] {(300, 0, 0), (300, 0, 300), (300, 300, 300), (300, 300, 0)});
            var roof = new Wall(new Vector3D[] {(0, 0, 300), (0, 300, 300), (300, 300, 300), (300, 0, 300)});
            var floor = new Wall(new Vector3D[] {(0, 0, 0), (0, 300, 0), (300, 300, 0), (300, 0, 0)});
            var room = new Room(new[] {rightWall, leftFal, backWall, frontWall, roof, floor}, 0.5);
            room.Start(Line.FromPointAndDirection((1.5, 0, 1.5), (0, 1.7, 2.4)));

            var imageMatrix = new short[300, 300];
            var xCords = rightWall.HittedPoints.Select(x => x.X).ToList();
            var zCords = rightWall.HittedPoints.Select(x => x.Z).ToList();
            for (var i = 0; i < xCords.Count; ++i)
            {
                var x = (int) (Math.Round(xCords[i], 2) * 100);
                var y = (int) (Math.Round(zCords[i], 2) * 100);
                imageMatrix[x, y]++;
            }
            Console.ReadKey();
        }
    }
}