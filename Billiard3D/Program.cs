using System;
using System.Collections.Concurrent;
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
    public class Program
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
            //ParallelStart(0.05);
            //CylinderCaustic();

            var alpha = new double[] {0, 45, 89};
            const int from = 0;
            const int to = 3;
            Parallel.For(from, to, i => { OctoSphereCaustic(alpha[i]); });


            //var alpha = new double[] { 0, 45, 89 };
            //   const int from = 0;
            //   const int to = 3;
            //   Parallel.For(from, to, i => { CylinderCaustic(alpha[i]); });


            //var alpha = new double[]{ 0, 45, 89 };
            //const int from = 0;
            //const int to = 3;
            //Parallel.For(from, to, i => { SphereCaustic2(alpha[i]); });
        }

        private static void CylinderCaustic(double alpha)
        {
            var startLines = CreateCylinderStartLines(alpha, 500, 100, 4).ToList();
            foreach (var startLine in startLines)
            {
                var causticCylinder = new CausticCylinder(4);
                var line = causticCylinder.Start(startLine);
                Writer(line, @"C:\Workspaces\etc\szakdoga\CAUSTICYLINDERMINE", $"PointTableQuarter{alpha}.txt");
            }
        }

        private static void SphereCaustic2(double alpha)
        {
            var startingPoints = SphereStart(alpha, 500, 500);
            foreach (var startingPoint in startingPoints)
            {
                var sphere2 = new CausticSphere();
                var line = sphere2.Start(startingPoint);
                Writer(line, @"C:\Workspaces\etc\szakdoga\CAUSTICSPHERE5", $"PointTable{alpha}.txt");
            }
        }

        private static void OctoSphereCaustic(double alpha)
        {
            var startingPoints = OctoSphereStart(alpha, 500, 500);
            foreach (var startingPoint in startingPoints)
            {
                var sphere2 = new CausticSphere(8);
                var line = sphere2.Start(startingPoint);
                Writer(line, @"C:\Workspaces\etc\szakdoga\CAUSTICSPHERE5", $"PointTableOctave{alpha}.txt");
            }
        }

        private static IEnumerable<Line> OctoSphereStart(double alpha, int xLimit, int yLimit)
        {
            var inRadian = alpha.ToRadian();
            var sideLine = new Line((2, 0, 2), (0, 2, 2));
            var sideDistance = Vector3D.AbsoluteValue(sideLine.SecondPoint - sideLine.BasePoint);
            var sideRange = Numpy.LinSpace(0, sideDistance, xLimit).ToList();
            var zRange = Numpy.LinSpace(0, 2, yLimit).ToList();
            foreach (var x in sideRange)
            {
                foreach (var z in zRange)
                {
                    if (x * x + z * z >= 0) continue;

                    var side = sideLine.GetPointOnLine(x);
                    var direction = (new Vector3D(0, 0, 1) - (1, 1, 1)).Normalize();
                    var rotatedDirection = new Vector3D(direction.X * Cos(inRadian) - direction.Y * Sin(inRadian),
                        direction.X * Sin(inRadian) + direction.Y * Cos(inRadian), direction.Z);
                    yield return Line.FromPointAndDirection((side.X, side.Y, z), rotatedDirection);
                }
            }
      }

        private static IEnumerable<Line> SphereStart(double alpha, int xLimit, int yLimit)
        {
            var inRadian = alpha.ToRadian();
            var dx = Tan(inRadian);
            var xRange = Numpy.LinSpace(-1, 1, xLimit).ToList();
            var yRange = Numpy.LinSpace(-1, 1, yLimit).ToList();
            foreach (var x in xRange)
            {
                foreach (var y in yRange)
                {
                    if (x * x + y * y >= 1) continue;

                    yield return Line.FromPointAndDirection((x - dx, y, 2), (dx, 0, -1));
                }
            }
        }

        private static IEnumerable<Line> CreateCylinderStartLines(double r)
        {
            const double dx = 0.5;
            const int dy = 1;
            var startX = -Sqrt(2) / 2 * r;
            var endX = Sqrt(2) / 2 * r;
            for (var x = startX; x < endX; x += dx)
            {
                for (var y = 0; y < 100; y += dy)
                {
                    yield return Line.FromPointAndDirection((x * Cos(45.0.ToRadian()), y, -x * Sin(45.0.ToRadian())),
                        new Vector3D(1, 0, 1).Normalize());
                }
            }
        }

        private static IEnumerable<Line> CreateCylinderStartLines(double alpha, int zLimit, int yLimit, double quarter)
        {
            var inRadian = alpha.ToRadian();
            var dz = Tan(inRadian);
            var r = Sqrt(Pow(CausticCylinder.R, 2) - Pow(CausticCylinder.R / quarter, 2));
            var zRange = Numpy.LinSpace(-r, r, zLimit).ToList();
            var yRange = Numpy.LinSpace(0, CausticCylinder.L, yLimit).ToList();
            foreach (var z in zRange)
            {
                foreach (var y in yRange)
                {
                    if (Abs(Abs(z) - Abs(r)) <= 5e-5 || Abs(y) <= 5e-5 || Abs(y - CausticCylinder.L) <= 5e-5) continue;
                    yield return Line.FromPointAndDirection((-CausticCylinder.R / quarter, y, z), (-1, 0, dz));
                }
            }
        }

        private static void LimesRun()
        {
            var radii = new[]
                {50, 100, 150, 200, 250, 300}; //{0.01, 0.02, 0.03, 0.04, 0.05, 0.06, 0.07, 0.08, 0.09, 0.1};
            var startingPoint = CreateStartingPoints(1).First();
            const string directoryPath = @"C:\Workspaces\etc\szakdoga\LongWithSmallRadii10";
            Parallel.ForEach(radii, radius =>
            {
                var room = TrackFactory.RoomWithPlaneRoof(radius);
                room.NumberOfIterations = 1_000_000;
                room.Start(startingPoint);

                var specificPath = Path.Combine(directoryPath, radius.ToString(CultureInfo.InvariantCulture));
                Writer(room, specificPath, FileMode.Create);
            });
        }

        private static void Writer(Line line, string directory, string name)
        {
            Directory.CreateDirectory(directory);
            var fullPath = Path.Combine(directory, name);
            using (var fileSteam = new FileStream(fullPath, FileMode.Append, FileAccess.Write))
            {
                using (var writer = new StreamWriter(fileSteam))
                {
                    lock (LockObject)
                    {
                        writer.WriteLine($"{line.BasePoint}\t{line.Direction}");
                    }
                }
            }
        }

        private static void Writer(IEnumerable<Vector3D> hitPoints, string directory, FileMode mode)
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
                            lock (LockObject)
                            {
                                writer.WriteLine(word.ToString());
                            }
                        }
                    }
                }
            }

            const string sequenceName = "Caustic.txt";

            WriteToFile(sequenceName, hitPoints);
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
                            lock (LockObject)
                            {
                                writer.WriteLine(word.ToString());
                            }
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
            var startingPoints = VeryCloseStartingPoints(1);
            var distances = new List<List<Vector3D>>(1);
            Parallel.ForEach(startingPoints, (startLine, _, l) =>
            {
                var room = TrackFactory.RoomWithPlaneRoof(radius);
                room.NumberOfIterations = 100_000;
                room.Start(startLine);
                lock (LockObject)
                {
                    Console.WriteLine($"Done with {startLine.Direction}");
                    distances.Add(room.EveryHitpoint);
                }

                var directory = $@"C:\Workspaces\etc\szakdoga\kicsik2tized4\{l}";
                Directory.CreateDirectory(directory);
                Writer(room, directory, FileMode.Create);
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
            var startingPoints = CausticStartPoints(30.0.ToRadian());
            Console.WriteLine("Start points done");
            Parallel.ForEach(startingPoints, (startLine, _, i) =>
            {
                var room = new Caustic();
                var list = room.Start(startLine);
                var name = startLine.BasePoint.ToString();
                var directory = "C:\\Workspaces\\etc\\szakdoga\\KAUSZTIKA2\\" + name;
                if (list.Count > 0)
                    Writer(list, directory, FileMode.Append);
            });
        }

        private static IEnumerable<Line> CausticStartPoints(double angle)
        {
            var points = new List<Line>();
            for (var i = 25; i <= 75; ++i)
            {
                for (var j = 25; j <= 75; ++j)
                {
                    var startPoint = (50, i, j);
                    Vector3D direction1 = (-1, i, j);
                    Vector3D direction2 = (-1, 25 * Sin(angle) + i, j);
                    points.Add(Line.FromPointAndDirection(startPoint, direction1));
                    points.Add(Line.FromPointAndDirection(startPoint, direction2));
                }
            }

            return points;
        }

        private static IEnumerable<Line> CausticStartPoints()
        {
            var points = new ConcurrentBag<Line>();
            Parallel.For(-10, 10, i =>
            {
                for (var j = -10; j < 10; ++j)
                {
                    for (var y = 1; y < 10; ++y)
                    {
                        for (var z = 1; z < 10; ++z)
                        {
                            points.Add(Line.FromPointAndDirection((99, y, z),
                                new Vector3D(-1, 0, 0) + new Vector3D(0, i, j)));
                        }
                    }
                }
            });
            return points;
        }

        private static void WriteCaustic(List<Vector3D> list, string name, long l)
        {
            if (list.Count < 1)
                return;
            var path = $@"C:\Workspaces\etc\szakdoga\kausztikus\{l}\";
            Directory.CreateDirectory(path);
            lock (LockObject)
            {
                using (var fs = new FileStream(path + $@"\{name}.txt", FileMode.Create, FileAccess.Write))
                {
                    using (var writer = new StreamWriter(fs))
                    {
                        list.ForEach(x => writer.WriteLine(x));
                    }
                }
            }
        }

        private static void WriteCaustic(List<Vector3D> list, long l, string name)
        {
            if (list.Count < 1)
                return;
            var path = $@"C:\Workspaces\etc\szakdoga\kausztikus\{l}\";
            Directory.CreateDirectory(path);
            lock (LockObject)
            {
                using (var fs = new FileStream(path + $@"\{name}{l}.txt", FileMode.Create, FileAccess.Write))
                {
                    using (var writer = new StreamWriter(fs))
                    {
                        list.ForEach(x => writer.WriteLine(x));
                    }
                }
            }
        }

        private static void WriteCausticWithOnlyLast(IReadOnlyCollection<Vector3D> list, string name, long l)
        {
            if (list.Count < 1)
                return;
            var path = $@"C:\Workspaces\etc\szakdoga\kausztikus\{l}\";
            Directory.CreateDirectory(path);
            lock (LockObject)
            {
                using (var fs = new FileStream(path + $@"\{name}Last.txt", FileMode.Create, FileAccess.Write))
                {
                    using (var writer = new StreamWriter(fs))
                    {
                        if (list.Count > 0)
                            writer.WriteLine(list.Last());
                    }
                }
            }
        }

        private static IEnumerable<Line> ParallelStartingPoints(Vector3D direction)
        {
            for (var y = 1; y < 100; ++y)
            {
                for (var z = 1; z < 100; ++z)
                {
                    yield return Line.FromPointAndDirection((99, y, z), (-1, 0, 0) + direction);
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
            var radii = new[] {0.02, 0.05, 0.007, 0.1, 0.2, 0.3, 0.4, 0.5, 0.6, 0.7, 0.8, 0.9, 1, 5, 50, 100};
            Parallel.ForEach(radii, (radius, _, l) =>
            {
                foreach (var line in startPoints)
                {
                    var room = TrackFactory.RoomWithPlaneRoof(radius);
                    room.NumberOfIterations = 100_000;
                    room.Start(line);
                    var folderPath = $@"C:\Workspaces\etc\szakdoga\radii\{l}";
                    Writer(room, folderPath, FileMode.Create);
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