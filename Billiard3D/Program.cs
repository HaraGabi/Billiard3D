using System;
using Billiard3D.Math;
using Billiard3D.Track;

namespace Billiard3D
{
    using JetBrains.Annotations;

    [UsedImplicitly]
    public class Program
    {
        public static void Main(string[] args)
        {
            var jobbFal = new Wall(new Vector3D[] { (0, 0, 0), (3, 0, 0), (0, 0, 3), (3, 0, 3) });
            var balFal = new Wall(new Vector3D[] { (0, 3, 0), (3, 3, 0), (0, 3, 3), (3, 3, 3) });
            var hátFal = new Wall(new Vector3D[] { (0, 0, 0), (0, 3, 0), (0, 3, 3), (0, 0, 3) });
            var szembeFal = new Wall(new Vector3D[] { (3, 0, 0), (3, 0, 3), (3, 3, 3), (3, 3, 0) });
            var tető = new Wall(new Vector3D[] { (0, 0, 3), (0, 3, 3), (3, 3, 3), (3, 0, 3) });
            var talaj = new Wall(new Vector3D[] { (0, 0, 0), (0, 3, 0), (3, 3, 0), (3, 0, 0) });
            var szoba = new Room(new Wall[] { jobbFal, balFal, hátFal, szembeFal, tető, talaj });
            szoba.StartSimulation((1.5, 0, 0), (2, 1, 1.4));

            Console.ReadKey();
        }
    }
}
