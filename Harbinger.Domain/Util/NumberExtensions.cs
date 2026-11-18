using System;
using System.Collections.Generic;
using System.Text;

namespace Harbinger.Domain.Util;

public static class NumberExtensions
{

    extension(decimal dec)
    {
        public int ToInt() => Convert.ToInt32(Math.Round(dec));
        public double ToDouble() => Convert.ToDouble(dec);

        public decimal Sqrt() => dec.ToDouble().Sqrt().ToDecimal();

        public decimal Pow2() => dec * dec;
    }

    extension(double d)
    {
        public decimal ToDecimal()
        {
            try
            {
                return Convert.ToDecimal(d);
            }
            catch(Exception)
            {
                Console.WriteLine($"Motherfudging crazy double: {d.ToString("N10")}");
                throw;
            }
        }
        public double Sqrt() => Math.Sqrt(d);
    }

    extension(int i)
    {
        public decimal ToDecimal() => i;

        public int DivideByWithDecimals(int other) => (i.ToDecimal() / (other.ToDecimal())).ToInt();

        public int MultiplyAndConvert(decimal other) => (i.ToDecimal() * other).ToInt();

    }

    extension<T>(IEnumerable<T> inp)
    {
        public int AvgWithDecimals(Func<T, int> extractor) => inp
            .Select(extractor)
            .Pipe(coll => coll.Sum().DivideByWithDecimals(coll.Count()));
    }
    extension(IEnumerable<int> inp)
    {
        public int AvgWithDecimals() => inp
            .Pipe(coll => coll.Sum().DivideByWithDecimals(coll.Count()));
    }

}
