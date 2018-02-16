using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Autrage.LEX.NET
{
    public static class Arith
    {
        public static float Avg(params float[] numbers) => numbers.Sum() / numbers.Length;
        public static double Avg(params double[] numbers) => numbers.Sum() / numbers.Length;
    }
}
