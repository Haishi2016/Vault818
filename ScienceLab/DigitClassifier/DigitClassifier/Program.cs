using MathNet.Numerics.LinearAlgebra;
using SharpNet;
using System;
using System.Collections.Generic;
using System.IO;
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
            const string dataRoot = @"D:\Haishi-2017\Vault818-Local\MNIST";

            //Load training data
            var trainingImages = MINSTDataLoader.LoadImages(Path.Combine(dataRoot, "train-images-idx3-ubyte.gz"));
            var trainingLabels = MINSTDataLoader.LoadLabels(Path.Combine(dataRoot, "train-labels-idx1-ubyte.gz"));
            var trainingSet = MINSTDataLoader.CombineImagesAndLabels(trainingImages, trainingLabels);

            //Load test data
            var testingImages = MINSTDataLoader.LoadImages(Path.Combine(dataRoot, "t10k-images-idx3-ubyte.gz"));
            var testingLabels = MINSTDataLoader.LoadLabels(Path.Combine(dataRoot, "t10k-labels-idx1-ubyte.gz"));
            var testingSet = MINSTDataLoader.CombineImagesAndLabels(testingImages, testingLabels);

            //Print training set information and a sample image
            Console.WriteLine("Training set size: " + trainingSet.Count);
            Console.WriteLine("Testing set size: " + testingSet.Count);
            int index = mRand.Next(trainingSet.Count);
            Console.WriteLine(string.Format("Here's a random picture from the training set: #{0} ({1})", index, trainingSet[index].Label));
            MINSTDataVisualizer.PrintImage(trainingSet[index].Image, 28);

            Network network;

            if (args.Length > 0)
            {
                using (StreamReader reader = new StreamReader(File.OpenRead(args[0])))
                {
                    network = Network.Load(reader);
                }
            }
            else
            {
                ////MSR
                //network = new Network(new QuadraticCost(), 784, 30, 10);
                ////Train the network
                //network.Train(trainingSet, 30, 10, 3, testingSet, 10);

                ////Cross-entropy
                //network = new Network(new CrossEntropyCost(), 784, 100, 10);
                ////Train the network
                //network.Train(trainingSet, 30, 10, 0.5, testingSet, 10);

                //Cross-entropy regulated
                network = new Network(new CrossEntropyCost(), 784, 100, 10);
                //Train the network
                network.Train(trainingSet, 120, 10, 0.1, testingSet, 10, 5.0);
            }

            //Now validate
            Console.WriteLine("\nNetwork is trained!");
            while (true)
            {
                Console.Write(string.Format("\nPlease enter a test image index [0 - {0}]. Enter '-1' to evaluate all test images. Enter '-2' to exit:", testingSet.Count - 1));
                if (int.TryParse(Console.ReadLine(), out index))
                {
                    if (index == -1)
                    {
                        int count = 0;
                        Console.Write(string.Format("Detecting {0} pictures", testingSet.Count));
                        for (int i = 0; i < testingSet.Count; i++)
                        {
                            var detection = network.Detect(CreateVector.Dense<double>(testingSet[i].Image));
                            if (detection == testingSet[i].Label)
                            {
                                Console.Write(".");
                                count++;
                            }
                            else
                                Console.Write("X");
                        }
                        Console.WriteLine("\nDetected {0} out of {1} pictures, correct rate is: {2:0.0%}", count, testingSet.Count, count * 1.0 / testingSet.Count);
                    }
                    else if (index >= 0 && index < testingSet.Count)
                    {
                        Console.WriteLine(string.Format("Test image: #{0} ({1})", index, testingSet[index].Label));
                        MINSTDataVisualizer.PrintImage(testingSet[index].Image, 28);
                        var detection = network.Detect(CreateVector.Dense<double>(testingSet[index].Image));
                        Console.WriteLine(string.Format("\nDetected number: {0} - {1}", detection, detection == testingSet[index].Label ? "SUCCESS!" : "FAIL!"));
                    }
                    else if (index == -2)
                        break;
                    else if (index == -3)
                    {
                        Console.Write("Please enter file name: ");
                        string fileName = Console.ReadLine();
                        using (StreamWriter writer = new StreamWriter(File.Create(fileName)))
                        {
                            network.Save(writer);
                        }
                    }
                }
            }
        }
    }
}
