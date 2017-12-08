using System;
using System.IO;
using System.Linq;
using Billiard3D.Track;
using Billiard3D.VectorMath;
using JetBrains.Annotations;

namespace Billiard3D
{
    [UsedImplicitly]
    public class Programs
    {
        private static Vector3D ChosenPoint { get; } = (320d, 0d, 0d);

        private static Vector3D InitialVelocity { get; } = (1d, 1d, 1d);

        public static void Main(string[] args)
        {
            var room = TrackFactory.RoomWithPlaneRoof(0.5);
            room.Start(CreateStartingPoint());

            WriteToFile(room, false, ChosenPoint.ToString(), InitialVelocity.ToString());
        }

        private static Line CreateStartingPoint() => Line.FromPointAndDirection(ChosenPoint, InitialVelocity);

        private static void WriteToFile(Room finished, bool isTilted, string startingPoint, string startingVelocity)
        {
            var rootDir = isTilted ? "Tilted" : "Common";
            rootDir += $@"\StartPoint {startingPoint} startVelocity {startingVelocity}";
            var directory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) +
                            $@"\{rootDir}\{finished.Radius}\";
            Directory.CreateDirectory(directory);
            foreach (var trackObject in finished.Objects)
            {
                var name = trackObject.ObjectName + ".txt";
                var fullPath = directory + name;
                File.WriteAllLines(fullPath,trackObject.HitPoints.Select(x => x.ToString()));
            }

            var metaDataName = directory + @"\StartingParameters.txt";
            var sequenceDataName = directory + @"\Sequence.txt";

            var newStartFormat = startingPoint.Trim('{', '}');
            var newStartVelocity = startingVelocity.Trim('{', '}');
            File.WriteAllLines(metaDataName, new[] {newStartFormat, newStartVelocity});
            File.WriteAllLines(sequenceDataName, finished.HitSequence);
        }
    }
}