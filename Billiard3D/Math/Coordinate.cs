using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Linq;

namespace Billiard3D.Math
{

    public class Coordinate : IEnumerable<double>, IEquatable<Coordinate>, IEqualityComparer<Coordinate>
    {
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

        public bool Equals(Coordinate x, Coordinate y) => x.Equals(y);

        public int GetHashCode(Coordinate obj) => X.GetHashCode() + Y.GetHashCode() + Z.GetHashCode();

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
    }
}
