using System;
using Billiard3D.VectorMath;
using JetBrains.Annotations;
using static System.Math;

namespace Billiard3D.Track
{
    internal class Sphere : IEquatable<Sphere>
    {
        private const double Confidence = 0.00005;

        public Sphere(Vector3D center, double radius)
        {
            Center = center;
            Radius = radius;
        }

        public Vector3D Center { get; }
        public double Radius { get; }

        public bool Equals(Sphere other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(Center, other.Center) && Radius.Equals(other.Radius);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return (obj.GetType() == GetType()) && Equals((Sphere) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Center != null ? Center.GetHashCode() : 0) * 397) ^ Radius.GetHashCode();
            }
        }

        private double Equation(Vector3D vector) => Pow(vector.X - Center.X, 2) +
                                                    Pow(vector.Y - Center.Y, 2) + Pow(vector.Z - Center.Z, 2);

        public bool OnSphere(Vector3D vector) => Abs(Equation(vector)) < Confidence;

        public (bool isInner, double) OnInnerSide(Vector3D vector, Vector3D referencePoint)
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

        public static bool operator ==([CanBeNull] Sphere lValue, [CanBeNull] Sphere rValue)
        {
            if (lValue is null) return false;
            if (rValue is null) return false;
            return (lValue.Center == rValue.Center) && (lValue.Radius - rValue.Radius < Confidence);
        }

        public static bool operator !=(Sphere lValue, Sphere rValue) => !(lValue == rValue);

        public (bool, double?) DoesHit(Vector3D directionVector, Vector3D origin, Vector3D referencePoint)
        {
            var unitDirection = directionVector.Normalize();
            var result = QuadraticFormula(unitDirection, origin, Center, Radius);
            if ((result.plus < 0) && (result.minus < 0))
                return (false, null);
            var witPlus = OnInnerSide(origin + result.plus * unitDirection, referencePoint);
            var withMinus = OnInnerSide(origin + result.minus * unitDirection, referencePoint);

            if (!witPlus.isInner && !withMinus.isInner)
                return (false, null);

            if (withMinus.isInner && witPlus.isInner)
                throw new ArgumentException("The algorithm is incorrect");

            return witPlus.isInner ? (true, result.plus) : (true, result.minus);

            (double plus, double minus) QuadraticFormula(Vector3D directionUnit, Vector3D originPoint, Vector3D center,
                double radius)
            {
                var firstPart = -(directionUnit * (origin - center));
                var oMinusC = originPoint - center;
                var determinant = Pow(directionVector * oMinusC, 2) - Pow(Vector3D.AbsoluteValue(oMinusC), 2) +
                                  Pow(radius, 2);
                if (determinant < 0) return (-1, -1);
                var plus = firstPart + Sqrt(determinant);
                var minus = firstPart - Sqrt(determinant);
                return (plus, minus);
            }
        }
    }
}