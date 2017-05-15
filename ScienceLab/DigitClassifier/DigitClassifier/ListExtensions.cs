using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitClassifier
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
        public static Vector<double> Vectorize(this IList<byte> list)
        {
            double[] array = new double[list.Count];
            for (int i = 0; i < list.Count; i++)
                array[i] = list[i] / 255.0;
            return CreateVector.DenseOfArray<double>(array);
        }
    }
}
