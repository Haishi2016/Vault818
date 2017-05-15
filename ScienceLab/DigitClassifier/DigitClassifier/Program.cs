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

            //Load test data
            var testingImages = MINSTDataLoader.LoadImages(@"C:\HaishiRooster\Data\MINST\t10k-images-idx3-ubyte.gz");
            var testingLabels = MINSTDataLoader.LoadLabels(@"C:\HaishiRooster\Data\MINST\t10k-labels-idx1-ubyte.gz");
            var testingSet = MINSTDataLoader.CombineImagesAndLabels(testingImages, testingLabels);

            //Print training set information and a sample image
            Console.WriteLine("Training set size: " + trainingSet.Count);
            Console.WriteLine("Testing set size: " + testingSet.Count);
            int index = mRand.Next(trainingSet.Count);
            Console.WriteLine(string.Format("Here's a random picture from the training set: #{0} ({1})", index, trainingSet[index].Label));
            MINSTDataVisualizer.PrintImage(trainingSet[index].Image, 28);

            //Train the network
            Network network = new Network(784, 30, 10);
            network.Train(trainingSet, 30, 10, 3.0, testingSet, 10);

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
                            var detection = network.Detect(CreateVector.Dense<double>(MINSTDataLoader.Normalize(testingSet[i].Image)));
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
                        var detection = network.Detect(CreateVector.Dense<double>(MINSTDataLoader.Normalize(testingSet[index].Image)));
                        Console.WriteLine(string.Format("\nDetected number: {0} - {1}", detection, detection == testingSet[index].Label ? "SUCCESS!" : "FAIL!"));
                    }
                    else if (index == -2)
                        break;
                }
            }
        }
    }
}
