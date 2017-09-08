using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpNet
{
    public class VectorMath
    {
        public static Vector<double> Sigmoid(Vector<double> z)
        {
            return 1.0 / (1.0 + Vector<double>.Exp(-z));
        }
        public static Vector<double> SigmoidPrime(Vector<double> z)
        {
            return Sigmoid(z).PointwiseMultiply(1 - Sigmoid(z));
        }
        public static Vector<double> GenerateMaskVector(int size, int[] masks)
        {
            var ret = CreateVector.Dense<double>(size, 1.0);
            var matrix = ret.ToRowMatrix();
            matrix.ClearColumns(masks);
            return matrix.Row(0);
        }
        public static Vector<double> GenerateComplementaryVector(int size, int[] mask)
        {
            var ret = CreateVector.Dense<double>(size, 1.0);
            var matrix = ret.ToRowMatrix();
            matrix = (matrix - 1).PointwiseAbs();
            return matrix.Row(0);
        }
        public static Vector<double> UpdateVectorWithMask(Vector<double> vector, Vector<double> newVector, Vector<double> mask, Vector<double> complement)
        {
            return vector.PointwiseMultiply(complement) + newVector.PointwiseMultiply(mask);
        }
    }
}
