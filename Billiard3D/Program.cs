﻿using System;
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
            var points = new[] { new Vector3D(0, 0, 0), new Vector3D(2, 0, 0), new Vector3D(0, 2, 0), new Vector3D(2, 2, 0) };
            var fal = new Wall(points);
            Console.WriteLine(fal.WasHit(new Vector3D(1, 1, 0)));
            fal.AngleAfterHit(new Vector3D(1, 1, 0));
            Console.ReadKey();
        }
    }
}
