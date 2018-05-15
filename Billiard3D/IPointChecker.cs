using Billiard3D.VectorMath;

namespace Billiard3D
{
   internal interface IPointChecker
   {
      bool IsPointOnTheCorrectSide(Vector3D point);
   }
}