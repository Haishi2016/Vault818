using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitClassifier
{
    public interface ICostFunction
    {
        Vector<double> Delta(Vector<double> activation, Vector<double> expected, Vector<double> weightedInput);
    }
}
