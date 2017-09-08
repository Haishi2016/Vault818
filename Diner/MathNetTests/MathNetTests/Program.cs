using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Complex;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MathNetTests
{
    class Program
    {
        static void Main(string[] args)
        {
            var matrix = CreateMatrix.DenseOfArray<double>(new double[,] { 
                { 1.0, -1.0, 2.0},
                { 0.0, -3.0, 1.0}});
            var vector = CreateVector.Dense<double>(new double[] { 2, 1, 0 });

            //Matrix-Vector product
            Console.WriteLine(MatrixDotVector(matrix, vector)); //Expected: [1 -3]

            //Hadamard Procut
            Console.WriteLine(HadamardProduct(vector, vector)); //Expected: [4,1,0]

            //Pointwise substraction
            Console.WriteLine(PointwiseSubstraction(vector, vector)); //Expected: [0,0,0]
            Console.WriteLine(PointwiseSubstraction(matrix, matrix)); //Expected: [0,0,0][0,0,0]

            //Mask a row/column (but not clear it)
            var uMatrix = CreateMatrix.Dense<double>(2, 3, 1.0);
            uMatrix.ClearColumns(0, 2);
            var cMatrix = CreateMatrix.Dense<double>(2, 3, 1.0);
            cMatrix.ClearColumn(1);
            //with 1st and 3rd columns masked
            var newMatrix = matrix.PointwiseMultiply(uMatrix);
            Console.WriteLine(newMatrix);
            //original matrix
            Console.WriteLine(matrix);
            //update the newMatrix
            newMatrix[0, 1] = 100;
            newMatrix[1, 1] = 200;
            Console.WriteLine(newMatrix);
            //push new values back to original
            matrix = matrix.PointwiseMultiply(cMatrix) + newMatrix.PointwiseMultiply(uMatrix);
            Console.WriteLine(matrix);

            //Vector equality
            Vector<double> a1 = CreateVector.DenseOfArray<double>(new double[]{ 1.2, 3.4, 5.6});
            Vector<double> a2 = CreateVector.DenseOfArray<double>(new double[] { 12, 34, 56 });
            Console.WriteLine(a1 == a2);
            Console.WriteLine(a1.Equals(a2));
            Console.WriteLine(a1.Normalize(1));
            Console.WriteLine(a1.Normalize(1).Equals(a2));
        }
        static Vector<double> MatrixDotVector(Matrix<double> matrix, Vector<double> vector)
        {
            return matrix * vector;
        }
        static Vector<double> HadamardProduct(Vector<double> a, Vector<double> b)
        {
            return a.PointwiseMultiply(b);
        }
        static Vector<double> PointwiseSubstraction(Vector<double> a, Vector<double> b)
        {
            return a - b;
        }
        static Matrix<double> PointwiseSubstraction(Matrix<double> a, Matrix<double> b)
        {
            return a - b;
        }
    }
}
