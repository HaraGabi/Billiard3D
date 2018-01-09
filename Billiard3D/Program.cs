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
            //ParallelSimulation();
            var start = Line.FromPointAndDirection((952.0446364170271, 132.49404079443096, 620.74585673165927),
                (0.082108154884514215, -0.8674675569011453, -0.49067126329694372));
            const int r = 100;
            var room = TrackFactory.RoomWithPlaneRoof(r);
            room.NumberOfIterations = 100;
            room.Start(start);

            var folderPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + @"\Teszt.txt";
            var tuples = room.CoordList.Select(x => x.Item1.ToString() + " " + x.Item2);
            File.WriteAllLines(folderPath, tuples);
            //var r = new[] { 200, 300};
            //foreach (var i in r)
            //{
            //    var room = TrackFactory.RoomWithPlaneRoof(i);
            //    room.Start(start);
            //    WriteToFile(room, false, start.PointA.ToString(), start.Direction.ToString());
            //}
        }

        private static void ParallelSimulation()
        {
            var options = new ParallelOptions {MaxDegreeOfParallelism = 4};
            var startingPoint = CreateStartingPoints().First();
            Parallel.For(1, 20, options, (index, state) =>
            {
                var radius = index * 2.5;
                try
                {
                    var room = TrackFactory.RoomWithPlaneRoof(radius);
                    room.Start(startingPoint);
                    WriteToFile(room, false, startingPoint.PointA.ToString(), startingPoint.Direction.ToString());
                }
                catch (Exception)
                {
                    lock (LockObject)
                    {
                        File.AppendAllLines(
                            Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + @"\Log.txt",
                            new[] {$"Hiba! {radius} - {startingPoint.PointA} - {startingPoint.Direction}"});
                        Console.WriteLine($"Error with {radius}");
                    }
                }
                lock (LockObject)
                {
                    Console.WriteLine($"Done with {index}");
                }
            });
        }

        private static IEnumerable<Line> CreateStartingPoints()
        {
            var result = new List<Line>();
            var velocities = new List<Vector3D>();
            velocities.AddRange(Enumerable.Repeat(0,15).Select(x => GetRandomVector()));
            result.AddRange(velocities.Select(x => Line.FromPointAndDirection(ChosenPoint, x)));
            return result;
        }

        private static Vector3D GetRandomVector()
        {
            return (Rand.Next(1, 10), Rand.Next(1, 10), Rand.Next(1, 10));
        }

        private static void WriteToFile(Room finished, bool isTilted, string startingPoint, string startingVelocity)
        {
            var rootDir = isTilted ? "Tilted" : "Common";
            rootDir += $@"\StartPoint {startingPoint} startVelocity {startingVelocity}";
            var directory = @"D:\szakdoga\adatok" + $@"\{rootDir}\{finished.Radius}\";
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