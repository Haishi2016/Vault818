using MathNet.Numerics.LinearAlgebra;
using SharpNet;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitClassifier
{
    class Program
    {
        static Random mRand = new Random();
        static int left = 0, top = 0;
        static void Main(string[] args)
        {
            const string dataRoot = @"C:\HaishiRooster\Data\MINST";

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
            Console.WriteLine(string.Format("Here's a random picture from the training set: #{0} ({1})", index, convertToByte(trainingSet[index].Label)));
            MINSTDataVisualizer.PrintImage(trainingSet[index].Image, 28);

            Network network;

            if (args.Length > 0)
            {
                using (StreamReader reader = new StreamReader(File.OpenRead(args[0])))
                {
                    network = Network.Load(reader);
                }
                Console.WriteLine("\nNetwork is loaded!");
                dumpHyperParameters(network.HyperParameters);
            }
            else
            {
                ////MSR
                //HyperParameters hyperParameters = new HyperParameters { CostFunctionName = "QuadraticCost", Epochs = 30, MiniBatchSize = 10, LearningRate = 3, TestSize = 10, AutoSave = true, AutoSaveThreshold = 0.951 };
                ////Cross-entropy
                HyperParameters hyperParameters = new HyperParameters { CostFunctionName = "CrossEntropyCost", Epochs = 30, MiniBatchSize = 10, LearningRate = 0.08, TestSize = testingSet.Count, AutoSave = true, AutoSaveThreshold = 0.967 };

                network = new Network(hyperParameters, 784, 30, 10);

                hookupEvents(network);
                dumpHyperParameters(hyperParameters);

                //Train the network
                network.Train(trainingSet, (actual, expected)=>
                {
                    return convertToByte(actual) == convertToByte(expected);
                }, testingSet);

                ////Cross-entropy regulated
                //network = new Network(new CrossEntropyCost(), 784, 100, 10);
                ////Train the network
                //network.Train(trainingSet, 30, 10, 0.1, testingSet, 10, 5.0);

                //Cross-entropy regulated with dropouts
                //network = new Network(new CrossEntropyCost(), 784, 200, 10);
                //Train the network
                //network.Train(trainingSet, 120, 10, 0.1, testingSet, 500, 5.0, useDropouts:true, saveBestRun:true, runTarget:0.982);
                Console.WriteLine("\nNetwork is trained!");
            }

            //Now validate

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
                            var detection = network.Detect(testingSet[i].Image);
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
                        Console.WriteLine(string.Format("Test image: #{0} ({1})", index, convertToByte(testingSet[index].Label)));
                        MINSTDataVisualizer.PrintImage(testingSet[index].Image, 28);
                        var detection = network.Detect(testingSet[index].Image);
                        Console.WriteLine(string.Format("\nDetected number: {0} - {1}", detection, convertToByte(detection) == convertToByte(testingSet[index].Label) ? "SUCCESS!" : "FAIL!"));
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
        private static byte convertToByte(Vector<double> result)
        {
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

        private static void hookupEvents(Network network)
        {
            //Hook up events for monitoring
            network.OnEpochStart += (o, e) =>
            {
                Console.WriteLine("\nRunning training epoch " + e.Epoch);
                Console.Write("   Training...");
                Console.CursorVisible = false;
                left = Console.CursorLeft;
                top = Console.CursorTop;
            };
            network.OnEpochValidationStart += (o, e) =>
            {
                Console.WriteLine("   Validating " + e.TestSize + " test pictures...");
            };
            network.OnEpochValidationEnd += (o, e) =>
            {
                Console.WriteLine("   Detected {0} out of {1} pictures, correct rate is: {2:0.0%}", e.DataSize, e.TestSize, e.DetectionRate);
                using (StreamWriter writer = File.AppendText("logs.txt"))
                {
                    writer.Write(e.DetectionRate);
                    writer.Write(",");
                }
            };
            network.OnAutoSaveRequest += (o, e) =>
            {
                Console.WriteLine("   Saving...");
                NumberFormatInfo nfi = new NumberFormatInfo();
                nfi.NumberDecimalSeparator = "-";
                using (StreamWriter writer = new StreamWriter(File.Create(DateTime.Now.ToString("yyyy-MM-dd-HH-mm-") + e.Epoch + "-" + e.DetectionRate.ToString(nfi) + ".txt")))
                {
                    network.Save(writer);
                }
            };
            network.OnMiniBatchStart += (o, e) =>
            {
                Console.SetCursorPosition(left, top);
                Console.Write(string.Format("{0:P2}", e.Sample / (e.DataSize * 1.0 / e.BatchSize)));
            };
            network.OnMiniBatchEnd += (o, e) =>
            {
                Console.SetCursorPosition(left, top);
                Console.WriteLine("100.00 %");
                Console.CursorVisible = true;
            };
        }
        private static void dumpHyperParameters(HyperParameters hyperParameters)
        {
            Console.WriteLine("");
            Console.WriteLine("                   Hyper Parameters");
            Console.WriteLine("======================================================");
            Console.WriteLine("                          Epochs: " + hyperParameters.Epochs);
            Console.WriteLine("                 Mini Batch Size: " + hyperParameters.MiniBatchSize);
            Console.WriteLine("                   Cost Function: " + hyperParameters.CostFunctionName);
            Console.WriteLine("                   Learning Rate: " + hyperParameters.LearningRate);
            Console.WriteLine("                    Use Dropouts: " + hyperParameters.UseDropouts);
            Console.WriteLine("Hidden Layer Dropout Probability: " + hyperParameters.HiddenLayerDropoutProbability);
            Console.WriteLine(" Input Layer Dropout Probability: " + hyperParameters.InputLayerDropoutProbability);
            Console.WriteLine("               Regulation Lambda: " + hyperParameters.RegulationLambda);
            Console.WriteLine("                       Test Size: " + hyperParameters.TestSize);
            Console.WriteLine("                       Auto-save: " + hyperParameters.AutoSave);
            Console.WriteLine("             Auto-save Threshold: " + hyperParameters.AutoSaveThreshold);
            Console.WriteLine("======================================================");
            Console.WriteLine("");
        }
    }
}
