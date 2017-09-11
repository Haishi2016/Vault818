using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpNet
{
    public class MatrixMath
    {
        private static Random mRand = new Random();
        public static int[] PickMaskedElements(int count, double probability)
        {
            int[] tmp = new int[count];
            for (int i = 0; i < tmp.Length; i++)
                tmp[i] = i;
            List<int> chosen = new List<int>();

            for (int i = 0; i < tmp.Length; i++)
                if (mRand.NextDouble() > probability)
                    chosen.Add(tmp[i]);
            if (chosen.Count == count)
                chosen.RemoveAt(0);
            chosen.Sort();
            return chosen.ToArray();
        }
        public static Matrix<double> GenerateMaskingMatrix(int rows, int cols, int[] mask, bool vertical)
        {
            var matrix = CreateMatrix.Dense<double>(rows, cols, 1.0);
            if (vertical)
                matrix.ClearColumns(mask);
            else
                matrix.ClearRows(mask);
            return matrix;
        }
        public static Matrix<double> GenerateComplementaryMarix(int rows, int cols, int[] mask, bool vertical)
        {
            var matrix = CreateMatrix.Dense<double>(rows, cols, 1.0);
            if (vertical)
                matrix.ClearColumns(mask);
            else
                matrix.ClearRows(mask);
            matrix = (matrix - 1).PointwiseAbs();
            return matrix;
        }
        public static Matrix<double> UpdateMatrixWithMask(Matrix<double> matrix, Matrix<double> newMatrix, Matrix<double> maskMatrix, Matrix<double> complementMatrix)
        {
            return matrix.PointwiseMultiply(complementMatrix) + newMatrix.PointwiseMultiply(maskMatrix);
        }
    }
}
