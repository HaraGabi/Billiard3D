namespace Billiard3D.Track
{
   using System;
   using System.Collections.Generic;
   using System.Linq;

   using Billiard3D.VectorMath;

   using JetBrains.Annotations;

   internal class Room
   {
      #region Constructors and Destructors

      public Room([NotNull] IEnumerable<Wall> walls, double radius = 0.003)
      {
         if (walls == null) throw new ArgumentNullException(nameof(walls));
         Radius = radius;
         var index = 0;
         foreach (var wall in walls)
         {
            var name = index.ToString();
            wall.ObjectName = name;
            Objects.Add(wall);
            index++;
         }

         CreateCylinders(radius);
         CreateSpheres(radius);
      }

      #endregion

      #region Public Properties

      public List<Vector3D> EveryHitpoint { get; } = new List<Vector3D>();

      public List<string> HitSequence { get; } = new List<string>();

      public List<Vector3D> NonWallHitpoints { get; } = new List<Vector3D>();

      public int NumberOfIterations { private get; set; } = 10_000_000;

      public List<ITrackObject> Objects { get; } = new List<ITrackObject>(24);

      public double Radius { get; }

      #endregion

      #region Properties

      private double MinimumWallDistance { get; set; }

      #endregion

      #region Public Methods and Operators

      public void Start(Line startLine)
      {
         var currentLine = startLine;
         foreach (var item in Objects)
         {
            if (item.IsInCorrectPosition(startLine))
               throw new ArgumentException(item.ObjectName);
         }

         for (var i = 0; i < NumberOfIterations; ++i)
         {
            var hitPoints = Objects.Select(x => (x, x.GetIntersectionPoints(in currentLine))).Where(x => x.Item2.Any())
               .Select(x => (x.x, x.Item2.OrderByDescending(v => Vector3D.AbsoluteValue(v - currentLine.PointA)).First())).ToList();
            var hitted = hitPoints.OrderBy(x => Selector(x.x)).First();
            HitSequence.Add(hitted.x.ObjectName);
            if (!(hitted.x is Wall))
               NonWallHitpoints.Add(hitted.Item2);

            EveryHitpoint.Add(hitted.Item2);
            currentLine = hitted.x.LineAfterHit(in currentLine, in hitted.Item2);
         }
      }

      #endregion

      #region Methods

      [Pure]
      private static int AddSphere(List<Sphere> spheres, int index, Sphere sphere)
      {
         if (spheres.Exists(x => x.Center == sphere.Center))
            return index;

         var name = "S" + index;
         sphere.ObjectName = name;
         spheres.Add(sphere);
         index++;

         return index;
      }

      [Pure]
      private static int Selector(ITrackObject trackObject)
      {
         switch (trackObject)
         {
            case Sphere _: return 0;
            case Cylinder _: return 1;
            case Wall _: return 2;
            default: throw new NotSupportedException();
         }
      }

      private Cylinder CalculateCylinder(Line first, Line second, Line wallLine, double radius)
      {
         var angle = Vector3D.Angle(first.Direction, second.Direction);
         var distance = Math.Sin(90.0.ToRadian()) / Math.Sin(angle / 2) * radius;
         MinimumWallDistance = Math.Sqrt(Math.Pow(distance, 2) - Math.Pow(radius, 2));

         var referencePoint = wallLine.Contains(first.PointA) ? first.PointA : first.PointB;
         var firstSign = first.PointA == referencePoint ? +1 : -1;
         var secondSign = second.PointA == referencePoint ? +1 : -1;

         var firstPoint = first.GetPointOnLine(referencePoint, firstSign * MinimumWallDistance);
         var secondPoint = second.GetPointOnLine(referencePoint, secondSign * MinimumWallDistance);

         var crossLine = new Line(firstPoint, secondPoint);
         var directLine = new Line(referencePoint, crossLine.ClosestPoint(referencePoint));
         var top = directLine.GetPointOnLine(distance);

         var difference = top - referencePoint;
         var otherPoint = referencePoint == wallLine.PointA ? wallLine.PointB : wallLine.PointA;
         var bottom = otherPoint + difference;

         var basLine = new Line(top, bottom);
         var reverseBase = new Line(bottom, top);

         var correctTop = basLine.GetPointOnLine(radius);
         var correctBottom = reverseBase.GetPointOnLine(radius);

         var wallHeight = Vector3D.AbsoluteValue(wallLine.PointB - wallLine.PointA);
         var sign = otherPoint == wallLine.PointA ? -1 : +1;
         var bottomPoint = firstPoint + sign * wallHeight * (wallLine.PointB - wallLine.PointA).Normalize();

         var barrier = new Plane(firstPoint, secondPoint, bottomPoint);
         var checker = new PointChecker(barrier, barrier.DeterminePointPosition(referencePoint));

         return new Cylinder(correctTop, correctBottom, checker, radius);
      }

      private void CreateCylinders(double radius)
      {
         var cylinders = new List<Cylinder>(12);
         foreach (var track in Objects)
         {
            var wall = track as Wall ?? throw new ArgumentNullException(nameof(Objects));
            for (var index = 0; index < wall.WallLines.Count; index++)
            {
               var wallLine = wall.WallLines[index];
               var otherWall = Objects.Except(new[] { wall }).Cast<Wall>().ToList().Single(x => x.WallLines.Exists(line =>
                  ((line.PointA == wallLine.PointA) && (line.PointB == wallLine.PointB))
                  || ((line.PointA == wallLine.PointB) && (line.PointB == wallLine.PointA))));

               var topLine = wall.WallLines.Single(line =>
                  ((line.PointA == wallLine.PointA) && (line.PointB != wallLine.PointB))
                  || ((line.PointA == wallLine.PointB) && (line.PointB != wallLine.PointA)));

               var otherTopLine = otherWall.WallLines.Except(new[] { new Line(wallLine.PointB, wallLine.PointA), wallLine }).Single(x =>
                  (x.PointA == topLine.PointA) || (x.PointA == topLine.PointB) || (x.PointB == topLine.PointA) || (x.PointB == topLine.PointB));

               var cylinder = CalculateCylinder(topLine, otherTopLine, wallLine, radius);
               if (cylinders.Exists(x => x.Contains(cylinder.TopCenter) && x.Contains(cylinder.BottomCenter)))
                  continue;
               var name = "C" + index;
               cylinder.ObjectName = name;

               cylinders.Add(cylinder);
            }
         }

         Objects.AddRange(cylinders);
      }

      private Sphere CreateSphereBarrier([NotNull] Sphere sphere)
      {
         if (sphere == null) throw new ArgumentNullException(nameof(sphere));

         var walls = Objects.Where(x => x is Wall).Cast<Wall>().GroupBy(x => x.Corners.Select(y => Vector3D.AbsoluteValue(y - sphere.Center)).Min())
            .OrderBy(x => x.Key).SelectMany(x => x).Take(3).ToList();
         var corner = walls.Select(x => x.Corners).Aggregate((x, y) => x.Intersect(y).ToList()).Single();

         var intersectionPoints = walls.Select(sphere.GetInterSection).ToList();
         var barrier = new Plane(intersectionPoints[0], intersectionPoints[1], intersectionPoints[2]);
         var sign = barrier.DeterminePointPosition(corner);
         var checker = new PointChecker(barrier, sign);

         sphere.Checker = checker;
         return sphere;
      }

      private void CreateSpheres(double radius)
      {
         var cylinders = Objects.OfType<Cylinder>();
         var spheres = new List<Sphere>(8);
         var index = 0;
         foreach (var cylinder in cylinders)
         {
            var sphereA = CreateSphereBarrier(new Sphere(cylinder.TopCenter, radius));
            var sphereB = CreateSphereBarrier(new Sphere(cylinder.BottomCenter, radius));

            index = AddSphere(spheres, index, sphereA);
            index = AddSphere(spheres, index, sphereB);
         }

         Objects.AddRange(spheres);
      }

      #endregion
   }
}