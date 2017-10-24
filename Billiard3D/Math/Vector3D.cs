using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using static System.Math;

namespace Billiard3D.Math
{
    internal class Vector3D : IEnumerable<int>, IEnumerable, IComparable<Vector3D>, IEquatable<Vector3D>
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Z { get; set; }

        public Vector3D(int x, int y, int z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public Vector3D(IEnumerable<int> coll)
        {
            if (coll.Count() != 3)
            {
                throw new ArgumentException("This vector class only support 3D vectors");
            }
            var list = coll.ToArray();
            X = list[0];
            Y = list[1];
            Z = list[2];
        }

        public void Deconstruct(out int x, out int y, out int z)
        {
            x = X;
            y = Y;
            z = Z;
        }

        public IEnumerator<int> GetEnumerator()
        {
            yield return X;
            yield return Y;
            yield return Z;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public static double AbsoluteValue(Vector3D vector)
        {
            var squred = vector.Aggregate((sum, x) => sum += x * x);
            return Sqrt(squred);
        }

        public int CompareTo(Vector3D other)
        {
            if (AbsoluteValue(other) > AbsoluteValue(this))
            {
                return -1;
            }
            if (AbsoluteValue(other) < AbsoluteValue(this))
            {
                return 1;
            }
            return 0;
        }

        public bool Equals(Vector3D other)
        {
            if (this.SequenceEqual(other)) return true;
            return false;
        }

        public override bool Equals(object obj)
        {
            if (obj is Vector3D other)
                return Equals(other);
            return false;
        }

        public override int GetHashCode() => base.GetHashCode();

        public override string ToString() => $"{{{X}, {Y}, {Z}}}";

        public static Vector3D operator +(Vector3D lValue, Vector3D rValue)
        {
            var sum = lValue.Zip(rValue, (first, second) => first + second);
            return new Vector3D(sum);
        }

        public static Vector3D operator -(Vector3D lValue, Vector3D rValue)
        {
            var diff = lValue.Zip(rValue, (first, second) => first - second);
            return new Vector3D(diff);
        }

        public static double operator *(Vector3D lValue, Vector3D rValue)
        {
            var mult = lValue.Zip(rValue, (first, second) => first * second);
            return mult.Sum();
        }

        public static Vector3D operator *(Vector3D lValue, int rValue)
        {
            var mult = lValue.Select(x => x * rValue);
            return new Vector3D(mult);
        }

        public static Vector3D operator *(int lValue, Vector3D rValue) => rValue * lValue;

        public static implicit operator Vector3D((int x, int y, int z) tuple) => new Vector3D(tuple.x, tuple.y, tuple.z);

        public static implicit operator (int x, int y, int z)(Vector3D vector) => (vector.X, vector.Y, vector.Z);

        public static bool operator ==(Vector3D lValue, Vector3D rValue) => lValue.Equals(rValue);
        public static bool operator !=(Vector3D lValue, Vector3D rValue) => lValue.Equals(rValue);
    }
}
