using Billiard3D.VectorMath;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using static System.Math;

namespace Billiard3D.Track
{
    internal class Sphere
    {
        public Vector3D Coordinates { get; }
        public double Radius { get; }

        public Sphere(Vector3D coordinates, double radius)
        {
            Coordinates = coordinates;
            Radius = radius;
        }

        private double Equation(Vector3D vector)
        {
            return Pow((vector.X - Coordinates.X), 2) + Pow((vector.Y - Coordinates.Y), 2) + Pow((vector.Z - Coordinates.Z), 2);
        }

        public bool OnSphere(Vector3D vector)
        {
            return Abs(Equation(vector)) < 0.00005;
        }

        public bool OnInnerSpaher(Vector3D vector, Vector3D referencePoint)
        {
            return OnSphere(vector) && vector.Zip(Coordinates, (first, second) => first > second).Count() >= 2;
        }
    }
}
