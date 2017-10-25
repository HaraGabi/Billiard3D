using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Linq;

namespace Billiard3D.Math
{

    public class Coordinate : IEnumerable<double>, IEquatable<Coordinate>
    {
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            return (obj.GetType() == GetType()) && Equals((Coordinate)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = X.GetHashCode();
                hashCode = (hashCode * 397) ^ Y.GetHashCode();
                hashCode = (hashCode * 397) ^ Z.GetHashCode();
                return hashCode;
            }
        }

        public double X { get; }
        public double Y { get; }
        public double Z { get; }

        public Coordinate(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public void Deconstruct(out double x, out double y, out double z)
        {
            x = X;
            y = Y;
            z = Z;
        }
        public IEnumerator<double> GetEnumerator()
        {
            yield return X;
            yield return Y;
            yield return Z;
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public bool Equals(Coordinate other) => this.SequenceEqual(other);

        public double this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0: return X;
                    case 1: return Y;
                    case 2: return Z;
                        default: throw new ArgumentException("Indexer has to be between 0-2");
                }
            }
        }

        public static bool operator ==(Coordinate lValue, Coordinate rValue) => (lValue != null) && lValue.Equals(rValue);

        public static bool operator !=(Coordinate lValue, Coordinate rValue) => (lValue == null) || !lValue.Equals(rValue);
    }
}
