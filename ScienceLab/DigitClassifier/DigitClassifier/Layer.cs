using MathNet.Numerics.Distributions;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Complex;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitClassifier
{
    public class Layer
    {
        private Vector<double> mBias;
        private Matrix<double> mWeights;
        public Layer(int size, int sizeofPreviousLayer)
        {
            var distribution = new Normal(0.0, 1.0);
            mBias = CreateVector.Random<double>(size, distribution);
            if (sizeofPreviousLayer > 0)
                mWeights = CreateMatrix.Random<double>(size, sizeofPreviousLayer, distribution);
        }
        public (Vector<double> activation, Vector<double> weightedInput) FeedForward(Vector<double> activations)
        {
            if (mWeights != null)
            {
                var weighted = mWeights * activations + mBias;
                return (VectorMath.Sigmoid(weighted), weighted);
            }
            else
                return (activations, activations);
        }

        public Matrix<double> Weights
        {
            get
            {
                return mWeights;
            }
        }
        public void AdjustWeights(Matrix<double> delta)
        {
            mWeights = mWeights - delta;
        }
        public void AdjustBias(Vector<double> delta)
        {
            mBias = mBias - delta;
        }
    }
}
