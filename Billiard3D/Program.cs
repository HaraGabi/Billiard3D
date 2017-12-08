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
        internal static Vector3D ChosenPoint { get; } = (320d, 0d, 0d);

        public static void Main(string[] args)
        {
            var room = TrackFactory.RoomWithPlaneRoof(0.5);
            var objects = room.Start(Line.FromPointAndDirection(ChosenPoint, (1d, 1d, 1d)));

            var sequence = room.HitSequence;
            Console.ReadKey();
        }
    }
}