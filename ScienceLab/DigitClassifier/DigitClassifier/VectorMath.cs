using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitClassifier
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
    }
}
