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
