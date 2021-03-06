﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Billiard3D.VectorMath
{
    public static class Numpy
    {
        public static IEnumerable<double> Arrange(double start, int count)
        {
            return Enumerable.Range((int)start, count).Select(v => (double)v);
        }

        public static IEnumerable<double> Power(IEnumerable<double> exponents, double baseValue = 10.0d)
        {
            return exponents.Select(v => Math.Pow(baseValue, v));
        }

        public static IEnumerable<double> LinSpace(double start, double stop, int num, bool endpoint = true)
        {
            var result = new List<double>();
            if (num <= 0)
            {
                return result;
            }

            if (endpoint)
            {
                if (num == 1)
                {
                    return new List<double> { start };
                }

                var step = (stop - start) / (num - 1.0d);
                result = Arrange(0, num).Select(v => v * step + start).ToList();
            }
            else
            {
                var step = (stop - start) / num;
                result = Arrange(0, num).Select(v => v * step + start).ToList();
            }

            return result;
        }

        public static IEnumerable<double> LogSpace(double start, double stop, int num, bool endpoint = true, double numericBase = 10.0d)
        {
            var y = LinSpace(start, stop, num, endpoint);
            return Power(y, numericBase);
        }
    }
}
