using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitClassifier
{
    public class Network
    {
        private Layer[] mLayers;
        private ICostFunction mCostFunction;

        public Network(string costFunctioName)
        {
            mCostFunction = new QuadraticCost();
        }
        public Network(params int[] nodeCounts)
            :this(new QuadraticCost(), nodeCounts)
        {

        }
        public Network(ICostFunction costFunction, params int[] nodeCounts)
        {
            mCostFunction = costFunction;
            if (nodeCounts == null || nodeCounts.Length == 0)
                throw new ArgumentNullException();
            mLayers = new Layer[nodeCounts.Length];
            for (int i = 0; i < mLayers.Length; i++)
                mLayers[i] = new Layer(nodeCounts[i], i==0?0: nodeCounts[i-1]);
        }

        public void Train(List<(byte[] Image, byte Label)> data, int epochs, int miniBatchSize, double learningRate, List<(byte[] Image, byte Label)> testData = null, int testSize = 10)
        {
            if (data == null)
                throw new ArgumentNullException();
            int traingSetSize = data.Count;
            if (miniBatchSize <= 0 || miniBatchSize > traingSetSize)
                throw new ArgumentException("Mini-batch size needs to a positive value smaller than training set size.");

            for (int i = 0; i < epochs; i++)
            {
                Console.WriteLine("Running training epoch " + i);
                Console.WriteLine("   Training...");
                data.Shuffle();
                for (int j = 0; j < traingSetSize / miniBatchSize; j++)
                {
                    updateMiniBatch(data, j * miniBatchSize, miniBatchSize, learningRate);
                }
                if (testData != null)
                {
                    Console.WriteLine("   Validating " + testSize + " test pictures...");
                    int count = 0;
                    Random rand = new Random();
                    int startIndex = rand.Next(0, testData.Count - testSize + 1);
                    for (int v = 0; v < testSize; v++)
                    {
                        var detection = this.Detect(CreateVector.Dense<double>(MINSTDataLoader.Normalize(testData[v+startIndex].Image)));
                        if (detection == testData[v+startIndex].Label)
                            count++;
                    }
                    Console.WriteLine("   Detected {0} out of {1} pictures, correct rate is: {2:0.0%}", count, testSize, count * 1.0 / testSize);
                }
            }
        }
        private void updateMiniBatch(List<(byte[] Image, byte Label)> data, int startIndex, int batchSize, double leariningRate)
        {
            (Vector<double> nabulaB, Matrix<double> nabulaW)[] layeredSigmas = new(Vector<double> nabulaB, Matrix<double> nabulaW)[mLayers.Length];
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

           
            for (int l = mLayers.Length - 1; l >= 1; l--)
            {
                mLayers[l].AdjustBias(leariningRate / batchSize * layeredSigmas[l].nabulaB);
                mLayers[l].AdjustWeights(leariningRate / batchSize * layeredSigmas[l].nabulaW);
            }
        }
        private (Vector<double>[] nabulaB, Matrix<double>[] nabulaW) backPropagation((byte[] Image, byte Label) data)
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
            var expected = toResultVector(data.Label);
            var error = mCostFunction.Delta(activations[layerCount-1], expected, weightedInputs[layerCount - 1]);

            
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
        
        public Vector<double> FeedForward(Vector<double> input)
        {
            for (int i = 0; i < mLayers.Length; i++)
                input = mLayers[i].FeedForward(input).activation;
            return input;
        }

        public byte Detect(Vector<double> input)
        {
            var result = this.FeedForward(input);
            double max = double.MinValue;
            byte index = byte.MaxValue;
            for (byte i = 0; i <result.Count; i++)
            {
                if (result[i] >= max)
                {
                    max = result[i];
                    index = i;
                }
            }
            return index;
        }
        private Vector<double> costDerivative(Vector<double> activations, Vector<double> expected)
        {
            return activations - expected;
        }

        private Vector<double> toResultVector(byte label)
        {
            return CreateVector.DenseOfArray<double>(
                new double[]
                {
                    label == 0? 1.0:0.0,
                    label == 1? 1.0:0.0,
                    label == 2? 1.0:0.0,
                    label == 3? 1.0:0.0,
                    label == 4? 1.0:0.0,
                    label == 5? 1.0:0.0,
                    label == 6? 1.0:0.0,
                    label == 7? 1.0:0.0,
                    label == 8? 1.0:0.0,
                    label == 9? 1.0:0.0
                }
                );
        }
        public static Network Load(StreamReader reader)
        {
            var strContent = reader.ReadLine();
            var values = strContent.Split(';');
            string costFunction = values[0];
            int layerCount = int.Parse(values[1]);

            Layer[] layers = new Layer[layerCount];
            for (int i = 0; i < layers.Length; i++)
                layers[i] = Layer.Load(reader);

            Network network = new Network(costFunction);
            network.LoadLayers(layers);
            return network;
        }

        public void Save(StreamWriter writer)
        {
            writer.WriteLine(mCostFunction.GetType().Name + ";" + mLayers.Length);
            for (int i = 0; i < mLayers.Length; i++)
                mLayers[i].Save(writer);
        }

        public void LoadLayers(Layer[] layers)
        {
            this.mLayers = layers;
        }
    }
}
