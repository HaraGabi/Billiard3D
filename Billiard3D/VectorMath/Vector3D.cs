using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using static System.Math;

namespace Billiard3D.VectorMath
{
    [DebuggerDisplay("({X}; {Y}; {Z})")]
    internal readonly struct Vector3D : IEnumerable<double>, IComparable<Vector3D>, IEquatable<Vector3D>
    {
        public double X { get; }

        public double Y { get; }

        public double Z { get; }

        public Vector3D(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        private Vector3D(IEnumerable<double> coll)
        {
            var doubles = coll as double[] ?? coll.ToArray();
            if (doubles.Length != 3)
                throw new ArgumentException("This vector class only support 3D vectors");
            X = doubles[0];
            Y = doubles[1];
            Z = doubles[2];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int CompareTo(Vector3D other)
        {
            if (AbsoluteValue(other) > AbsoluteValue(this))
                return -1;
            return AbsoluteValue(other) < AbsoluteValue(this) ? 1 : 0;
        }

        public IEnumerator<double> GetEnumerator()
        {
            yield return X;
            yield return Y;
            yield return Z;
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(Vector3D other)
        {
            const double confidence = 0.00005;
            var first = Abs(X - other.X) < confidence;
            var second = Abs(Y - other.Y) < confidence;
            var third = Abs(Z - other.Z) < confidence;
            return first && second && third;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double AbsoluteValue(Vector3D vector)
        {
            var squared = vector.Select(x => x * x).Aggregate((sum, x) => sum + x);
            return Sqrt(squared);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Angle(Vector3D lValue, Vector3D rValue)
        {
            if (IsNullVector(lValue) || IsNullVector(rValue))
                return 90.0.ToRadian();
            var absL = AbsoluteValue(lValue);
            var absR = AbsoluteValue(rValue);
            return Acos(lValue * rValue / (absR * absL));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3D CrossProduct(Vector3D left, Vector3D right)
        {
            var a = left.Y * right.Z - left.Z * right.Y;
            var b = left.Z * right.X - left.X * right.Z;
            var c = left.X * right.Y - left.Y * right.X;
            return (a, b, c);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3D operator +(Vector3D lValue, Vector3D rValue)
        {
            var sum = lValue.Zip(rValue, (first, second) => first + second);
            return new Vector3D(sum);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3D operator /(Vector3D lValue, double rValue)
        {
            var div = lValue.Select(x => x / rValue);
            return new Vector3D(div);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3D operator /(double lValue, Vector3D rValue) => rValue / lValue;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(Vector3D lValue, Vector3D rValue) => lValue.Equals(rValue);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator Vector3D((double x, double y, double z) tuple) =>
            new Vector3D(tuple.x, tuple.y, tuple.z);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator (double x, double y, double z)(Vector3D vector) =>
            (vector.X, vector.Y, vector.Z);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(Vector3D lValue, Vector3D rValue) => !lValue.Equals(rValue);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double operator *(Vector3D lValue, Vector3D rValue)
        {
            var zip = lValue.Zip(rValue, (first, second) => first * second);
            return zip.Sum();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3D operator *(Vector3D lValue, int rValue)
        {
            var coll = lValue.Select(x => x * rValue);
            return new Vector3D(coll);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3D operator *(int lValue, Vector3D rValue) => rValue * lValue;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3D operator *(double lValue, Vector3D rValue)
        {
            var coll = rValue.Select(x => x * lValue);
            return new Vector3D(coll);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3D operator *(Vector3D lValue, double rValue) => rValue * lValue;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3D operator -(Vector3D lValue, Vector3D rValue)
        {
            var diff = lValue.Zip(rValue, (first, second) => first - second);
            return new Vector3D(diff);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Deconstruct(out double x, out double y, out double z)
        {
            x = X;
            y = Y;
            z = Z;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override bool Equals(object obj)
        {
            if (obj is Vector3D other)
                return Equals(other);
            return false;
        }

        public override int GetHashCode()
        {
            var hashCode = -307843816;
            hashCode = hashCode * -1521134295 + X.GetHashCode();
            hashCode = hashCode * -1521134295 + Y.GetHashCode();
            hashCode = hashCode * -1521134295 + Z.GetHashCode();
            return hashCode;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Vector3D Normalize() => this / AbsoluteValue(this);

        public override string ToString() => $"{X} {Y} {Z}";

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsNullVector(Vector3D vector) => vector.All(x => Abs(x) <= double.Epsilon);
    }

    public static class DoubleExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double ToRadian(this double degree) => PI * degree / 180.0;
    }
}