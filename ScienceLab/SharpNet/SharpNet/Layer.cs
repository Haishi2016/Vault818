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
        private Vector<double> mVectorMask;
        private double mProbability = 1.0;
        private bool mUseDropouts = false;
        int[] mSelection;

        public Layer(int size, int sizeofPreviousLayer) : this(CreateVector.Random<double>(size, new Normal(0.0, 1.0)),
                            sizeofPreviousLayer > 0 ? CreateMatrix.Random<double>(size, sizeofPreviousLayer, new Normal(0.0, 1.0/ Math.Sqrt(sizeofPreviousLayer))) : null)
        {
        }
        public Layer(Vector<double> bias, Matrix<double> weights)
        {
            mBias = bias;
            mWeights = weights;
        }
        public void Reset()
        {
            mBias = CreateVector.Random<double>(mBias.Count, new Normal(0.0, 1.0));
            if (mWeights != null)
                mWeights = CreateMatrix.Random<double>(mWeights.RowCount, mWeights.ColumnCount, new Normal(0.0, 1.0 / Math.Sqrt(mWeights.ColumnCount)));
        }
        public void SetMasks(double probability)
        {
            mSelection = MatrixMath.PickMaskedElements(mBias.Count, mProbability);
            mProbability = probability;
            mVectorMask = VectorMath.GenerateMaskVector(mBias.Count, mSelection);
            UseDropouts = true;
        }

        public bool UseDropouts
        {
            get { return mUseDropouts; }
            set { mUseDropouts = value;  }
        }
       
        public (Vector<double> activation, Vector<double> weightedInput) FeedForward(Vector<double> activations)
        {
            if (mWeights != null)
            {
                if (UseDropouts)
                {
                    Vector<double> weighted = mWeights * activations + mBias; 
                    return (VectorMath.Sigmoid(weighted).PointwiseMultiply(mVectorMask), weighted);
                }
                else
                {
                    Vector<double> weighted = mWeights * activations + mBias;
                    return (VectorMath.Sigmoid(weighted), weighted);
                }

            }
            else
            {
                if (UseDropouts)
                    return (activations.PointwiseMultiply(mVectorMask), activations);
                else
                    return (activations, activations);
            }
        }

        public Matrix<double> Weights
        {
            get
            {
                if (UseDropouts)
                {
                    var temp = MatrixMath.GenerateMaskingMatrix(mWeights.RowCount, mWeights.ColumnCount, mSelection, false);
                    return mWeights.PointwiseMultiply(temp);
                }
                else
                    return mWeights;
            }
        }
        public Vector<double> Bias
        {
            get
            {
                if (UseDropouts)
                    return mBias.PointwiseMultiply(mVectorMask);
                else
                    return mBias;
            }
        }
        public Vector<double> Mask
        {
            get
            {
                return mVectorMask;
            }
        }
        public void AdjustWeights(Matrix<double> delta, double weighDecay)
        {
            ////if (mMatrixMask != null)
            ////    MatrixMath.UpdateMatrixWithMask(mWeights, mWeights * weighDecay - delta, mMatrixMask, mMatrixComMask);
            ////else
            //if (UseDropouts)
            //{
            //    var temp = MatrixMath.GenerateMaskingMatrix(mWeights.RowCount, mWeights.ColumnCount, mSelection, false);
            //    mWeights = mWeights * weighDecay - delta.PointwiseMultiply(temp);
            //}
            //else
                mWeights = mWeights * weighDecay - delta;
        }
        public void AdjustBias(Vector<double> delta)
        {
            //if (UseDropouts)
            //    mBias = mBias - delta.PointwiseMultiply(mVectorMask);
            //else
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
                        writer.Write(mWeights[y, x]);
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
