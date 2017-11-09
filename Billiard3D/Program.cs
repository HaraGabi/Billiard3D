using System;
using System.Linq;
using Billiard3D.Math;
using Billiard3D.Track;
using ImageSharp;
using JetBrains.Annotations;

namespace Billiard3D
{
    [UsedImplicitly]
    public class Programs
    {
        public static void Main(string[] args)
        {
            var jobbFal = new Wall(new Vector3D[] {(0, 0, 0), (3, 0, 0), (0, 0, 3), (3, 0, 3)});
            var balFal = new Wall(new Vector3D[] {(0, 3, 0), (3, 3, 0), (0, 3, 3), (3, 3, 3)});
            var hátFal = new Wall(new Vector3D[] {(0, 0, 0), (0, 3, 0), (0, 3, 3), (0, 0, 3)});
            var szembeFal = new Wall(new Vector3D[] {(3, 0, 0), (3, 0, 3), (3, 3, 3), (3, 3, 0)});
            var tető = new Wall(new Vector3D[] {(0, 0, 3), (0, 3, 3), (3, 3, 3), (3, 0, 3)});
            var talaj = new Wall(new Vector3D[] {(0, 0, 0), (0, 3, 0), (3, 3, 0), (3, 0, 0)});
            var szoba = new Room(new[] {jobbFal, balFal, hátFal, szembeFal, tető, talaj});
            szoba.StartSimulation((1.5, 0, 1.5), (0, 1.7, 2.4));

            var image = new Image(300, 300);
            var imageMatrix = new short[300, 300];
            var xCords = jobbFal.HittedPoints.Select(x => x.X).ToList();
            var zCords = jobbFal.HittedPoints.Select(x => x.Z).ToList();
            for (var i = 0; i < xCords.Count; ++i)
            {
                var x = (int) (System.Math.Round(xCords[i], 2) * 100);
                var y = (int) (System.Math.Round(zCords[i], 2) * 100);
                imageMatrix[x, y]++;
            }
            Console.ReadKey();
        }
    }
}