using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitClassifier
{
    class Program
    {
        static Random mRand = new Random();
        static void Main(string[] args)
        {
            //Load training data
            var trainingImages = MINSTDataLoader.LoadImages(@"C:\HaishiRooster\Data\MINST\train-images-idx3-ubyte.gz");
            var trainingLabels = MINSTDataLoader.LoadLabels(@"C:\HaishiRooster\Data\MINST\train-labels-idx1-ubyte.gz");
            var trainingSet = MINSTDataLoader.CombineImagesAndLabels(trainingImages, trainingLabels);

            //Print training set information and a sample image
            Console.WriteLine("Training set size: " + trainingSet.Count);
            int index = mRand.Next(trainingSet.Count);
            Console.WriteLine(string.Format("Here's a random picture from the set: #{0} ({1})", index, trainingSet[index].Label));
            MINSTDataVisualizer.PrintImage(trainingSet[index].Image, 28);

            //Train the network
            Network network = new Network(784, 30, 10);
            network.Train(trainingSet, 30, 10, 3.0, trainingSet, 10);
            //var result = network.FeedForward(CreateVector.DenseOfArray<double>(normalize(trainingImages[index])));
            //Console.WriteLine(result);
        }
        
    }
}
