using Billiard3D.VectorMath;
using static System.Math;

namespace Billiard3D.Track
{
    internal class Sphere
    {
        private const double Confidence = 0.00005;

        public Sphere(Vector3D center, double radius)
        {
            Center = center;
            Radius = radius;
        }

        public Vector3D Center { get; }
        public double Radius { get; }

        private double Equation(Vector3D vector) => Pow(vector.X - Center.X, 2) +
                                                    Pow(vector.Y - Center.Y, 2) + Pow(vector.Z - Center.Z, 2);

        public bool OnSphere(Vector3D vector) => Abs(Equation(vector)) < Confidence;

        public (bool, double) OnInnerSide(Vector3D vector, Vector3D referencePoint)
        {
            var result = Equation(vector);
            if (!(Abs(result) < Confidence))
                return (false, result);
            // on the sphere
            return Abs(Vector3D.AbsoluteValue(referencePoint) - Vector3D.AbsoluteValue(vector)) >
                   Abs(Vector3D.AbsoluteValue(referencePoint) - Vector3D.AbsoluteValue(Center))
                ? (true, result)
                : (false, result);
        }
    }
}