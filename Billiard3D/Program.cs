using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Billiard3D.Track;
using Billiard3D.VectorMath;
using JetBrains.Annotations;

namespace Billiard3D
{
    [UsedImplicitly]
    public class Programs
    {
        private static readonly object LockObject = new object();
        private static Vector3D ChosenPoint { get; } = (0d, 320d, 328d);
        private static Random Rand { get; } = new Random();

        [UsedImplicitly]
        public static void Main(string[] args)
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture("en-GB");

            //ParallelAutocorrelationSimulation();
           VariableStartingPoint();
        }

        private static void ParallelAutocorrelationSimulation(double radius = 50)
        {
            var startingPoints = CreateStartingPoints(400);
            Parallel.ForEach(startingPoints, startpoint =>
            {
                var room = TrackFactory.RoomWithPlaneRoof(radius);
                room.NumberOfIterations = 2_000;
                room.Start(startpoint);
                WriteSequence(room, false, startpoint.Direction.ToString());
                lock (LockObject)
                {
                    Console.WriteLine($"Done with {startpoint.Direction}");
                }
            });
        }

       private static void VariableStartingPoint()
       {
          var startingVelocity = new Vector3D(2, 1, 3);
          Parallel.For(0, 400, i =>
          {
             var startingPoint = ChosenPoint * Rand.NextDouble();
             var line = Line.FromPointAndDirection(startingPoint, startingVelocity);
             var room = TrackFactory.RoomWithPlaneRoof(50);
             room.NumberOfIterations = 200;
             room.Start(line);
             WriteToFile(room, false, startingPoint.ToString(), startingVelocity.ToString());
          });
       }



       private static void ParallelSimulation()
        {
            var startingPoint = CreateStartingPoints().First();
            var radiuses = new[] { 10, 50, 100, 300 };
            Parallel.ForEach(radiuses, (radius, state) =>
            {
                var room = TrackFactory.RoomWithPlaneRoof(radius);
                room.NumberOfIterations = 1_000_000;
                room.Start(startingPoint);
                var folderPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + $@"\TesztNoWalls{radius}.txt";
                var tuples = room.NonWallHitpoints.Select(x => x.ToString());
                File.WriteAllLines(folderPath, tuples);
                WriteToFile(room, false, startingPoint.PointA.ToString(), startingPoint.Direction.ToString());
                lock (LockObject)
                {
                    Console.WriteLine($"Done with {radius}");
                }
            });
        }

        private static IEnumerable<Line> CreateStartingPoints(int howMany = 15)
        {
            var result = new List<Line>();
            var velocities = new List<Vector3D>();
            velocities.AddRange(Enumerable.Repeat(0,howMany).Select(x => GetRandomVector()));
            result.AddRange(velocities.Select(x => Line.FromPointAndDirection(ChosenPoint, x)));
            return result;
        }

        private static Vector3D GetRandomVector() => (Rand.Next(1, 10), Rand.Next(1, 10), Rand.Next(1, 10));

        private static void WriteToFile(Room finished, bool isTilted, string startingPoint, string startingVelocity)
        {
            var rootDir = isTilted ? "Tilted" : "Common";
            rootDir += $@"\StartPoint {startingPoint} startVelocity {startingVelocity}";
            var directory = @"C:\szakdoga\adatokDirection" + $@"\{rootDir}\{finished.Radius}\";
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

        private static void WriteSequence(Room finished, bool isTilted, string startingVelocity)
        {
            var rootDir = isTilted ? "Tilted" : "Common";
            var directory = @"C:\szakdoga\adatok" + $@"\{rootDir}\{finished.Radius}\";
            Directory.CreateDirectory(directory);

            var sequenceDataName = directory + $@"\Sequence{startingVelocity}.txt";

            lock (LockObject)
            {
                File.WriteAllLines(sequenceDataName, finished.HitSequence);
            }
        }
    }
}