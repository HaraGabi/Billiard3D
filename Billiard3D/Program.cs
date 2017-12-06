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
        internal static Line RandomStarter(int max)
        {
            var random = new Random();
            var startLocation = (random.Next(max), random.Next(max), random.Next(max));
            var startDirection = (random.Next(max), random.Next(max), random.Next(max));
            return Line.FromPointAndDirection(startLocation, startDirection);
        }

        public static void Main(string[] args)
        {
            var rightWall = new Wall(new Vector3D[] {(0, 0, 0), (300, 0, 0), (300, 0, 300), (0, 0, 300)});
            var leftFal = new Wall(new Vector3D[] {(0, 300, 0), (300, 300, 0), (300, 300, 300), (0, 300, 300)});
            var backWall = new Wall(new Vector3D[] {(0, 0, 0), (0, 300, 0), (0, 300, 300), (0, 0, 300)});
            var frontWall = new Wall(new Vector3D[] {(300, 0, 0), (300, 0, 300), (300, 300, 300), (300, 300, 0)});
            var roof = new Wall(new Vector3D[] {(0, 0, 300), (0, 300, 300), (300, 300, 300), (300, 0, 300)});
            var floor = new Wall(new Vector3D[] {(0, 0, 0), (0, 300, 0), (300, 300, 0), (300, 0, 0)});
            var room = new Room(new[] {rightWall, leftFal, backWall, frontWall, roof, floor}, 0.5);
            var objects = room.Start(RandomStarter(150));
        }
    }
}