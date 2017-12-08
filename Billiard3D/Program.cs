using System;
using System.Linq;
using Billiard3D.Track;
using Billiard3D.VectorMath;
using JetBrains.Annotations;

namespace Billiard3D
{
    using System.IO;
    using System.Threading.Tasks;

    [UsedImplicitly]
    public class Programs
    {
        private static Vector3D ChosenPoint { get; } = (320d, 0d, 0d);

        private static Vector3D InitialVelocity { get; } = (1d, 1d, 1d);

        public static void Main(string[] args)
        {
            var room = TrackFactory.RoomWithPlaneRoof(0.5);
            var objects = room.Start(CreateStartingPoint());

            var sequence = room.HitSequence;
            Console.WriteLine("Finished");
            Console.ReadKey();
            WriteToFile(room, false);
            Console.ReadKey();
        }

        private static Line CreateStartingPoint() => Line.FromPointAndDirection(ChosenPoint, InitialVelocity);

        private static void WriteToFile(Room finished, bool isTilted)
        {
            var rootDir = isTilted ? "Tilted" : "Common";
            var directory = $@"{rootDir}\{finished.Radius}\";
            foreach (var trackObject in finished.Objects)
            {
                var name = trackObject.ObjectName + ".txt";
                var fullPath = directory + name;
                using (var fileStream = new FileStream(fullPath, FileMode.Create, FileAccess.Write))
                {
                    using (var writer = new StreamWriter(fileStream))
                    {
                        trackObject.HitPoints.ForEach(x => writer.WriteLine(x));
                    }
                }
            }
        }
    }
}