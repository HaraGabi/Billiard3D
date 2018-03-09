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
using static System.Math;

namespace Billiard3D
{
    [UsedImplicitly]
    public class Programs
    {
        private static Vector3D ChosenPoint { get; } = (15d, 320d, 328d);

        private static Random Rand { get; } = new Random();
        private static readonly object LockObject = new object();


        [UsedImplicitly]
        public static void Main(string[] args)
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture("en-GB");

            //NearAutoCorrelation(50);
            //Kausztika();
            //ParallelAutoCorrelationSimulation();
            //VariableStartingPoint();
            //ParallelSimulation();
            //VeryLong();
            //LimesRun();
            ParallelStart(50);
        }

        private static void LimesRun()
        {
            var radii = new[] {0.01, 0.02, 0.03, 0.04, 0.05, 0.06, 0.07, 0.08, 0.09, 0.1};
            var startingPoint = CreateStartingPoints(1).First();
            const string directoryPath = @"C:\Workspaces\etc\szakdoga\LongWithSmallRadii";
            Parallel.ForEach(radii, radius =>
            {
                var room = TrackFactory.RoomWithPlaneRoof(radius);
                room.NumberOfIterations = 10_000_000;
                room.Start(startingPoint);

                var specificPath = Path.Combine(directoryPath, radius.ToString(CultureInfo.InvariantCulture));
                Writer(room, specificPath, FileMode.Create);
            });
        }

        private static void Writer(Room room, string directory, FileMode mode)
        {
            Directory.CreateDirectory(directory);

            void WriteToFile<T>(string name, IEnumerable<T> toWrite)
            {
                var fullPath = Path.Combine(directory, name);
                using (var fileStream = new FileStream(fullPath, mode, FileAccess.Write))
                {
                    using (var writer = new StreamWriter(fileStream))
                    {
                        foreach (var word in toWrite)
                        {
                            writer.WriteLine(word.ToString());
                        }
                    }
                }
            }

            const string sequenceName = "HitSequence.txt";
            const string everyHitPointName = "HitPoints.txt";
            
            WriteToFile(sequenceName, room.HitSequence);
            WriteToFile(everyHitPointName, room.EveryHitpoint);
            room.Boundaries.ForEach(x => WriteToFile(x.BoundaryName + ".txt", x.HitPoints));
        }

        private static IEnumerable<Line> CreateStartingPoints(int howMany = 15)
        {
            var result = new List<Line>();
            var velocities = new List<Vector3D>();
            velocities.AddRange(Enumerable.Repeat(0, howMany).Select(x => GetRandomVector()));
            result.AddRange(velocities.Select(x => Line.FromPointAndDirection(ChosenPoint, x)));
            return result;
        }

        private static Vector3D GetRandomVector()
        {
            var phi = (Rand.NextDouble() * 360).ToRadian();
            var theta = (Rand.NextDouble() * 180).ToRadian();
            var (x, y, z) = (Sin(theta) * Cos(phi), Sin(theta) * Sin(phi), Cos(theta));
            return (x, y, z);
        }

        private static void NearAutoCorrelation(double radius)
        {
            var startingPoints = VeryCloseStartingPoints(2);
            var distances = new List<List<Vector3D>>();
            Parallel.ForEach(startingPoints, (startLine, _, l) =>
            {
                var room = TrackFactory.RoomWithPlaneRoof(radius);
                room.NumberOfIterations = 2_000;
                room.Start(startLine);
                WriteSequence(room, false, l.ToString());
                lock (LockObject)
                {
                    Console.WriteLine($"Done with {startLine.Direction}");
                    distances.Add(room.EveryHitpoint);
                }
            });
            Directory.CreateDirectory(@"C:\Users\haraszti\Desktop\szakdoga\NearAuto2\kozel\");
            var distance = distances[0].Zip(distances[1], (x, y) => Vector3D.AbsoluteValue(x - y));
            File.WriteAllLines(@"C:\Users\haraszti\Desktop\szakdoga\NearAuto2\kozel\dist.txt",
                distance.Select(x => x.ToString(CultureInfo.CurrentCulture)));
        }

        private static void ParallelStart(double radius)
        {
            var startingPoints = VeryCloseStartingPoints(15);
            var distances = new List<List<Vector3D>>(15);
            Parallel.ForEach(startingPoints, (startLine, _, l) =>
            {
                var room = TrackFactory.RoomWithPlaneRoof(radius);
                room.NumberOfIterations = 2_000;
                room.Start(startLine);
                lock (LockObject)
                {
                    Console.WriteLine($"Done with {startLine.Direction}");
                    distances.Add(room.EveryHitpoint);
                    const string directory = @"C:\Workspaces\etc\szakdoga\kicsik";
                    Directory.CreateDirectory(directory);
                    var path = Path.Combine(directory, l + ".txt");
                    File.WriteAllLines(path, room.EveryHitpoint.Select(x => x.ToString()));
                }
            });
        }

        private static IEnumerable<Line> VeryCloseStartingPoints(int howMany)
        {
            var startingDirection = (1, 1, 0);
            for (var i = 0; i < howMany; ++i)
            {
                yield return Line.FromPointAndDirection(ChosenPoint + i * (Vector3D) (1e-10, 1e-10, 1e-10),
                    startingDirection);
            }
        }

        private static void Kausztika()
        {
            var startingPoints = ParallelStartingPoints();
            Parallel.ForEach(startingPoints, (startLine, _, i) =>
            {
                var room = new Caustic();
                var list = room.Start(startLine);
                WriteCaustic(list);
                WriteCaustic(list, i);
                WriteCausticWithOnlyLast(list);
            });
        }

        private static void WriteCaustic(List<Vector3D> list)
        {
            const string path = @"C:\Workspaces\etc\szakdoga\proba";
            Directory.CreateDirectory(path);
            lock (LockObject)
            {
                using (var fs = new FileStream(path + @"\Caustic.txt", FileMode.Append, FileAccess.Write))
                {
                    using (var writer = new StreamWriter(fs))
                    {
                        list.ForEach(x => writer.WriteLine(x));
                    }
                }
            }
        }

        private static void WriteCaustic(List<Vector3D> list, long l)
        {
            const string path = @"C:\Workspaces\etc\szakdoga\numberedSmall";
            Directory.CreateDirectory(path);
            lock (LockObject)
            {
                using (var fs = new FileStream(path + $@"\Caustic{l}.txt", FileMode.Append, FileAccess.Write))
                {
                    using (var writer = new StreamWriter(fs))
                    {
                        list.ForEach(x => writer.WriteLine(x));
                    }
                }
            }
        }

        private static void WriteCausticWithOnlyLast(List<Vector3D> list)
        {
            const string path = @"C:\Workspaces\etc\szakdoga";
            Directory.CreateDirectory(path);
            lock (LockObject)
            {
                using (var fs = new FileStream(path + @"\CausticLast.txt", FileMode.Append, FileAccess.Write))
                {
                    using (var writer = new StreamWriter(fs))
                    {
                        if (list.Count > 0)
                            writer.WriteLine(list.Last());
                    }
                }
            }
        }

        private static IEnumerable<Line> ParallelStartingPoints()
        {
            for (var y = 1; y < 100; ++y)
            {
                for (var z = 1; z < 100; ++z)
                {
                    yield return Line.FromPointAndDirection((99, y, z), (-1, 0, 0));
                }
            }
        }

        private static void ParallelAutoCorrelationSimulation(double radius = 50)
        {
            var startingPoints = CreateStartingPoints(400);
            Parallel.ForEach(startingPoints, (startLine, _, l) =>
            {
                var room = TrackFactory.RoomWithPlaneRoof(radius);
                room.NumberOfIterations = 2_000;
                room.Start(startLine);
                //WriteSequence(room, false, l.ToString());
                lock (LockObject)
                {
                    Console.WriteLine($"Done with {startLine.Direction}");
                }
            });
        }

        private static void ParallelSimulation()
        {
            var startingPoint = CreateStartingPoints().First();
            var radii = new[] {10, 50, 100, 300};
            Parallel.ForEach(radii, (radius, state) =>
            {
                var room = TrackFactory.RoomWithPlaneRoof(radius);
                room.NumberOfIterations = 100_000_000;
                room.Start(startingPoint);
                WriteToFile(room, false, startingPoint.BasePoint.ToString(), startingPoint.Direction.ToString());
                lock (LockObject)
                {
                    Console.WriteLine($"Done with {radius}");
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

        private static void VeryLong()
        {
            var startPoints = CreateStartingPoints(1).ToList(); //5000
            var radii = new[] { 60 };  //{0.02, 0.05, 0.007, 0.1, 0.2, 0.3, 0.4, 0.5, 0.6, 0.7, 0.8, 0.9, 1, 5}; //50,100
            Parallel.ForEach(radii, radius =>
            {
                foreach (var line in startPoints)
                {
                    var room = TrackFactory.RoomWithPlaneRoof(radius);
                    room.NumberOfIterations = 100_000;
                    room.Start(line);
                    var folderPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) +
                                     $@"\TestLong{radius}";
                    lock (LockObject)
                    {
                        //Directory.CreateDirectory(folderPath);
                        Directory.CreateDirectory(@"C:\Users\haraszti\Desktop\szakdoga\march07\varh");
                        var filePath = $@"C:\Users\haraszti\Desktop\szakdoga\march07\varh\seq{radius}.txt";
                        var filePath2 = $@"C:\Users\haraszti\Desktop\szakdoga\march07\varh\HitSequence{radius}.txt";
                        File.WriteAllLines(filePath, room.EveryHitpoint.Select(x => x.ToString()));
                        File.WriteAllLines(filePath2, room.HitSequence);
                        //foreach (var obj in room.Boundaries)
                        //{
                        //    var filePath = folderPath + $@"\{obj.BoundaryName}.txt";
                        //    using (var file = new FileStream(filePath, FileMode.Append, FileAccess.Write))
                        //    {
                        //        using (var writer = new StreamWriter(file))
                        //        {
                        //            foreach (var hitPoint in obj.HitPoints)
                        //            {
                        //                writer.WriteLine(hitPoint);
                        //            }
                        //        }
                        //    }
                        //}
                    }
                }
            });
        }

        private static void WriteSequence(Room finished, bool isTilted, string startingVelocity)
        {
            var directory = @"C:\Workspaces\etc\szakdoga\autoCorrKEVES" + $@"\{finished.Radius}\";
            Directory.CreateDirectory(directory);

            var sequenceDataName = directory + $@"\Sequence{startingVelocity}.txt";

            lock (LockObject)
            {
                File.WriteAllLines(sequenceDataName, finished.HitSequence);
            }
        }

        private static void WriteToFile(Room finished, bool isTilted, string startingPoint, string startingVelocity)
        {
            var rootDir = isTilted ? "Tilted" : "Common";
            rootDir += $@"\StartPoint {startingPoint} startVelocity {startingVelocity}";
            var directory = @"C:\szakdoga\adatokDirection" + $@"\{rootDir}\{finished.Radius}\";
            Directory.CreateDirectory(directory);
            foreach (var trackObject in finished.Boundaries)
            {
                var name = trackObject.BoundaryName + ".txt";
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