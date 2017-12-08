using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Billiard3D.Track;
using Billiard3D.VectorMath;
using JetBrains.Annotations;

namespace Billiard3D
{
    [UsedImplicitly]
    public class Programs
    {
        private static Vector3D ChosenPoint { get; } = (320d, 0d, 0d);

        [UsedImplicitly]
        public static void Main(string[] args)
        {
            Parallel.For(1, 41, (index, state) =>
            {
                var radius = index * 0.5;
                foreach (var startingPoint in CreateStartingPoints())
                {
                    var room = TrackFactory.RoomWithPlaneRoof(radius);
                    room.Start(startingPoint);
                    WriteToFile(room, false, startingPoint.PointA.ToString(), startingPoint.Direction.ToString());
                }
            });
        }

        private static IEnumerable<Line> CreateStartingPoints()
        {
            var result = new List<Line>();
            var velocities = new List<Vector3D>
            {
                (0, 0, 1),
                (0, 1, 0),
                (0, 1, 1),
                (1, 0, 0),
                (1, 0, 1),
                (1, 1, 0),
                (1, 1, 1)
            };
            result.AddRange(velocities.Select(x => Line.FromPointAndDirection(ChosenPoint, x)));
            return result;
        }

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
                File.WriteAllLines(fullPath, trackObject.HitPoints.Select(x => x.ToString()));
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