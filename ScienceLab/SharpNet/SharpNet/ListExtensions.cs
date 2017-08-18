using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpNet
{
    public static class ListExtensions
    {
        public static void Shuffle<T>(this IList<T> list)
        {
            var rand = new Random((int)(DateTime.Now.Ticks & int.MaxValue));
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rand.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
        public static Vector<double> Vectorize(this IList<double> list)
        {
            double[] array = new double[list.Count];
            list.CopyTo(array, 0);
            return CreateVector.DenseOfArray<double>(array);
        }
    }
}
