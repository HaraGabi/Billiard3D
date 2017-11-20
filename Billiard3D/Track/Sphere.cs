using System.Linq;
using Billiard3D.VectorMath;
using static System.Math;

namespace Billiard3D.Track
{
    internal class Sphere
    {
        public Sphere(Vector3D coordinates, double radius)
        {
            Coordinates = coordinates;
            Radius = radius;
        }

        public Vector3D Coordinates { get; }
        public double Radius { get; }

        private double Equation(Vector3D vector) => Pow(vector.X - Coordinates.X, 2) +
                                                    Pow(vector.Y - Coordinates.Y, 2) + Pow(vector.Z - Coordinates.Z, 2);

        public bool OnSphere(Vector3D vector) => Abs(Equation(vector)) < 0.00005;

        public bool OnInnerSphere(Vector3D vector, Vector3D referencePoint)
        {
            return OnSphere(vector) && (vector.Zip(Coordinates, (first, second) => first > second).Count() >= 2);
        }
    }
}