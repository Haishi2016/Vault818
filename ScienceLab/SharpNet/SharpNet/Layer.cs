using MathNet.Numerics.Distributions;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Complex;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpNet
{
    public class Layer
    {
        private Vector<double> mBias;
        private Matrix<double> mWeights;
        private Matrix<double> mMask;
        private Vector<double> mVectorMask;
        private Matrix<double> mComplement;
        private Vector<double> mComplementVector;
        public Layer(int size, int sizeofPreviousLayer) : this(CreateVector.Random<double>(size, new Normal(0.0, 1.0)),
                            sizeofPreviousLayer > 0 ? CreateMatrix.Random<double>(size, sizeofPreviousLayer, new Normal(0.0, 1.0/ Math.Sqrt(sizeofPreviousLayer))) : null)
        {
        }
        public Layer(Vector<double> bias, Matrix<double> weights)
        {
            mBias = bias;
            mWeights = weights;
        }
        public void SetMasks(int[] mask, bool selCols)
        {
            if (mWeights != null)
            {
                mMask = MatrixMath.GenerateMaskingMatrix(mWeights.RowCount, mWeights.ColumnCount, mask, selCols);
                mComplement = MatrixMath.GenerateComplementaryMarix(mWeights.RowCount, mWeights.ColumnCount, mask, selCols);
                if (mBias.Count == mask.Length)
                {
                    mVectorMask = VectorMath.GenerateMaskVector(mBias.Count, mask);
                    mComplementVector = VectorMath.GenerateMaskVector(mBias.Count, mask);
                }
                else
                {
                    mVectorMask = null;
                    mComplementVector = null;
                }
            }
        }
        public void ClearMasks()
        {
            mMask = null;
            mComplement = null;
        }
        public (Vector<double> activation, Vector<double> weightedInput) FeedForward(Vector<double> activations, bool useDropout = false)
        {
            if (mWeights != null)
            {
                Vector<double> weighted = null;
                if (mMask != null && mVectorMask != null)
                    weighted = mWeights.PointwiseMultiply(mMask) * activations + mBias.PointwiseMultiply(mVectorMask);
                else if (mMask != null)
                    weighted = mWeights.PointwiseMultiply(mMask) * activations + mBias;
                else if (useDropout)
                    weighted = mWeights * 0.5 * activations + mBias;
                else 
                    weighted = mWeights * activations + mBias;
                return (VectorMath.Sigmoid(weighted), weighted);
            }
            else
                return (activations, activations);
        }

        public Matrix<double> Weights
        {
            get
            {
                if (mMask != null)
                    return mWeights.PointwiseMultiply(mMask);
                else
                    return mWeights;
            }
        }
        public void AdjustWeights(Matrix<double> delta, double weighDecay)
        {
            if (mComplement != null)
                mWeights = MatrixMath.UpdateMatrixWithMask( mWeights, mWeights * weighDecay - delta, mMask, mComplement);
            else
                mWeights = mWeights * weighDecay - delta;
        }
        public void AdjustBias(Vector<double> delta)
        {
            if (mComplementVector != null)
                mBias = VectorMath.UpdateVectorWithMask(mBias, mBias - delta, mVectorMask, mComplementVector);
            else
                mBias = mBias - delta;
        }

        public void Save(StreamWriter writer, bool trainedWithDropouts = false)
        {
            if (mWeights == null)
                writer.WriteLine(mBias.Count + ";0;0");
            else 
                writer.WriteLine(mBias.Count + ";" + mWeights.ColumnCount + ";" + mWeights.RowCount);

            var biasArray = mBias.ToArray<double>();
            for (int i = 0; i < biasArray.Length; i++)
            {
                writer.Write(biasArray[i]);
                writer.Write(";");
            }
            writer.WriteLine();
            if (mWeights != null)
            {
                for (int y = 0; y < mWeights.RowCount; y++)
                {
                    for (int x = 0; x < mWeights.ColumnCount; x++)
                    {
                        writer.Write(trainedWithDropouts? mWeights[y, x] * 0.5 : mWeights[y, x]);
                        writer.Write(";");
                    }
                    writer.WriteLine();
                }
            }
        }
        public static Layer Load(StreamReader reader)
        {
            var strContent = reader.ReadLine();
            var values = strContent.Split(';');
            int biascount = int.Parse(values[0]);
            int colCount = int.Parse(values[1]);
            int rowCount = int.Parse(values[2]);

            strContent = reader.ReadLine();
            values = strContent.Split(';');
            var biasArray = new double[biascount];
            for (int i = 0; i < biascount; i++)
                biasArray[i] = double.Parse(values[i]);
            var bias = CreateVector.DenseOfArray<double>(biasArray);

            if (colCount != 0 && rowCount != 0)
            {
                double[,] mValues = new double[rowCount, colCount];
                for (int y = 0; y < rowCount; y++)
                {
                    strContent = reader.ReadLine();
                    values = strContent.Split(';');
                    for (int x = 0; x < colCount; x++)
                    {
                        mValues[y, x] = double.Parse(values[x]);
                    }
                }
                var weights = CreateMatrix.DenseOfArray(mValues);
                return new Layer(bias, weights);
            }
            else
                return new Layer(bias, null);
        }
    }
}
