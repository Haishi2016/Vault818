using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpNet
{
    public class Network
    {
        private Layer[] mLayers;        
        private HyperParameters mParameters;
        private ICostFunction mCostFunction;

        public event EventHandler<TrainingEventArgs> OnEpochStart;
        public event EventHandler<TrainingEventArgs> OnEpochValidationStart;
        public event EventHandler<TrainingEventArgs> OnEpochValidationEnd;
        public event EventHandler<TrainingEventArgs> OnAutoSaveRequest;
        public event EventHandler<TrainingEventArgs> OnMiniBatchStart;
        public event EventHandler<TrainingEventArgs> OnMiniBatchEnd;

        public HyperParameters HyperParameters { get { return mParameters; } }

        public Network(HyperParameters hyperParameters, params int[] nodeCounts)
        {
            mParameters = hyperParameters;
            switch(mParameters.CostFunctionName)
            {
                case "QuadraticCost":
                    mCostFunction = new QuadraticCost();
                    break;
                case "CrossEntropyCost":
                    mCostFunction = new CrossEntropyCost();
                    break;
                default:
                    throw new ArgumentException(string.Format("Invalid cost function name:'{0}'", mParameters.CostFunctionName));
            }
            if (nodeCounts == null || nodeCounts.Length == 0)
                return;
            mLayers = new Layer[nodeCounts.Length];
            for (int i = 0; i < mLayers.Length; i++)
                mLayers[i] = new Layer(nodeCounts[i], i == 0 ? 0 : nodeCounts[i - 1]);
        }
        

        public void Train(List<(Vector<double> Image, Vector<double> Label)> data, Func<Vector<double>, Vector<double>, bool> areEqual, List<(Vector<double> Image, Vector<double> Label)> testData = null)
        {
            if (data == null)
                throw new ArgumentNullException();
            
            int traingSetSize = data.Count;
            if (mParameters.MiniBatchSize <= 0 || mParameters.MiniBatchSize > traingSetSize)
                throw new ArgumentException("Mini-batch size needs to a positive value smaller than training set size.");

            for (int i = 0; i < mParameters.Epochs; i++)
            {
                if (this.OnEpochStart != null)
                    this.OnEpochStart(this, new TrainingEventArgs(epoch:i));
                data.Shuffle();
               
                for (int j = 0; j < traingSetSize / mParameters.MiniBatchSize; j++)
                {
                    if (this.OnMiniBatchStart != null)
                        this.OnMiniBatchStart(this, new TrainingEventArgs(dataSize: traingSetSize, batchSize: mParameters.MiniBatchSize, sample: j));
                    updateMiniBatch(data, j * mParameters.MiniBatchSize, mParameters.MiniBatchSize, mParameters.LearningRate, mParameters.RegulationLambda, mParameters.UseDropouts);
                }
                if (this.OnMiniBatchEnd != null)
                    this.OnMiniBatchEnd(this, new TrainingEventArgs());
                if (mParameters.UseDropouts)
                {
                    for (int k = 1; k < mLayers.Length; k++)
                    {
                        mLayers[k].ClearMasks();
                    }
                }
                if (testData != null)
                {
                    if (this.OnEpochValidationStart != null)
                        this.OnEpochValidationStart(this, new TrainingEventArgs(testSize:mParameters.TestSize));
                    int count = 0;
                    Random rand = new Random();
                    int startIndex = rand.Next(0, testData.Count - mParameters.TestSize + 1);
                    for (int v = 0; v < mParameters.TestSize; v++)
                    {
                        var detection = this.Detect(testData[v + startIndex].Image);
                        if (areEqual(detection, testData[v + startIndex].Label))
                            count++;
                    }
                    var detectionRate = count * 1.0 / mParameters.TestSize;
                    if (this.OnEpochValidationEnd != null)
                        OnEpochValidationEnd(this, new TrainingEventArgs(testSize: mParameters.TestSize, dataSize:count, detectionRate: detectionRate));

                    if (mParameters.AutoSave && detectionRate >= mParameters.AutoSaveThreshold && this.OnAutoSaveRequest != null)
                        this.OnAutoSaveRequest(this, new TrainingEventArgs(epoch:i, detectionRate: detectionRate));
                }
            }
        }
        private void updateMiniBatch(List<(Vector<double> Image, Vector<double> Label)> data, int startIndex, int batchSize, double leariningRate, double regulationLambda, bool useDropout)
        {
            (Vector<double> nabulaB, Matrix<double> nabulaW)[] layeredSigmas = new(Vector<double> nabulaB, Matrix<double> nabulaW)[mLayers.Length];

            //doesn't support dropouts in multiple hidden layers yet
            if (useDropout)
            {
                bool vertical = false;
                int[] mask = MatrixMath.PickHalfElements(mLayers[1].Weights.RowCount);
                for (int i = 1; i < mLayers.Length; i++)
                {
                    mLayers[i].SetMasks(mask, vertical);
                    vertical = !vertical;
                }
            }
            else
            {
                for (int i = 1; i < mLayers.Length; i++)
                    mLayers[i].ClearMasks();
            }
                
            for (int i = startIndex; i <= startIndex + batchSize -1; i++)
            {
                var result = backPropagation(data[i]);
                for (int l = 1;  l < mLayers.Length; l++)
                {
                    if (layeredSigmas[l].nabulaB == null)
                        layeredSigmas[l].nabulaB = result.nabulaB[l];
                    else
                        layeredSigmas[l].nabulaB = layeredSigmas[l].nabulaB + result.nabulaB[l];
                    if (layeredSigmas[l].nabulaW == null)
                        layeredSigmas[l].nabulaW = result.nabulaW[l];
                    else
                        layeredSigmas[l].nabulaW = layeredSigmas[l].nabulaW + result.nabulaW[l];
                }
            }


            var weightDecay = (1 - leariningRate * regulationLambda / data.Count());

            for (int l = mLayers.Length - 1; l >= 1; l--)
            {
                mLayers[l].AdjustBias(leariningRate / batchSize * layeredSigmas[l].nabulaB);
                mLayers[l].AdjustWeights(leariningRate / batchSize * layeredSigmas[l].nabulaW, weightDecay);
            }
        }
        private (Vector<double>[] nabulaB, Matrix<double>[] nabulaW) backPropagation((Vector<double> Image, Vector<double> Label) data)
        {
            int layerCount = mLayers.Length;
            Vector<double>[] nablaB = new Vector<double>[layerCount];
            Matrix<double>[] nablaW = new Matrix<double>[layerCount];
            Vector<double>[] activations = new Vector<double>[layerCount];
            Vector<double>[] weightedInputs = new Vector<double>[layerCount];

            //Feedforward
            activations[0] = data.Image.Vectorize();
            for (int i = 1; i < layerCount; i++)
            {
                var result = mLayers[i].FeedForward(activations[i-1]);
                activations[i] = result.activation.Clone();
                weightedInputs[i] = result.weightedInput.Clone();
            }

            //Output error
            var error = mCostFunction.Delta(activations[layerCount-1], data.Label, weightedInputs[layerCount - 1]);

            
            nablaB[layerCount - 1] = error;
            nablaW[layerCount - 1] = error.ToColumnMatrix() * activations[layerCount - 2].ToRowMatrix();

            //Backpropagate the error
            for (int i = mLayers.Length-2; i >=1; i--)
            {
                var z = weightedInputs[i];
                var sp = VectorMath.SigmoidPrime(z);
                error = (mLayers[i + 1].Weights.Transpose() * error).PointwiseMultiply(sp);
                nablaB[i] = error;
                nablaW[i] = error.ToColumnMatrix() * activations[i - 1].ToRowMatrix();
            }

            return (nablaB, nablaW);
        }
        
        public Vector<double> FeedForward(Vector<double> input, bool useDropouts = false)
        {
            for (int i = 0; i < mLayers.Length; i++)
                input = mLayers[i].FeedForward(input, useDropouts).activation;
            return input;
        }

        public Vector<double> Detect(Vector<double> input)
        {
            return this.FeedForward(input, mParameters.UseDropouts);
        }
        private Vector<double> costDerivative(Vector<double> activations, Vector<double> expected)
        {
            return activations - expected;
        }

        public static Network Load(StreamReader reader)
        {
            var strContent = reader.ReadLine();
            var hyperParameters = HyperParameters.Deserialize(strContent);
            int layerCount = int.Parse(reader.ReadLine());
            Layer[] layers = new Layer[layerCount];
            for (int i = 0; i < layers.Length; i++)
                layers[i] = Layer.Load(reader);

            Network network = new Network(hyperParameters);
            network.LoadLayers(layers);
            return network;
        }

        public void Save(StreamWriter writer)
        {
            writer.WriteLine(mParameters.Serialize());
            writer.WriteLine(mLayers.Length);
            for (int i = 0; i < mLayers.Length; i++)
                mLayers[i].Save(writer, mParameters.UseDropouts);
        }

        public void LoadLayers(Layer[] layers)
        {
            this.mLayers = layers;
        }
    }
}
