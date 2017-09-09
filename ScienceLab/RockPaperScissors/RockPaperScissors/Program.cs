using MathNet.Numerics.LinearAlgebra;
using SharpNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RockPaperScissors
{
    class Program
    {
        static string[] Names = new  string[] { "Rock", "Paper", "Scissors", "Lizard", "Spock" };

        static void Main(string[] args)
        {
            Network network = new Network(new HyperParameters
            {
                CostFunctionName = "CrossEntropyCost",
                Epochs = 100, //try 20000
                MiniBatchSize = 3,
                LearningRate = 1
            }, 5, 5, 5);

            //Before Training
            Console.WriteLine("Before training");
            Console.WriteLine("===============\n");
            testRockPaperScissors(network);

            //Our training set is very small - basically it encodes the rock-paper-scissors rules: if paper, then scissors and so on.
            List<(Vector<double>, Vector<double>)> data = new List<(Vector<double>, Vector<double>)>
            {
                //rock, paper, scissors
                (makeRock(), makePaper()),
                (makePaper(), makeScissors()),
                (makeScissors(), makeRock()),
            };

            //Train the network
            Console.ReadLine();
            network.Train(data, null);

            Console.WriteLine("\nAfter training");
            Console.WriteLine("==============\n");
            testRockPaperScissors(network);

            //Now train with Rock-Paper-Scissors-Lizard-Spock data
            //Our training set is very small - basically it encodes the rock-paper-scissors rules: if paper, then scissors and so on.
            data = new List<(Vector<double>, Vector<double>)>
            {
                //rock, paper, scissors
                (makeRock(), makePaper()),
                (makeRock(), makeSpock()),
                (makePaper(), makeScissors()),
                (makePaper(), makeLizard()),
                (makeScissors(), makeRock()),
                (makeScissors(), makeSpock()),
                (makeSpock(), makeLizard()),
                (makeSpock(), makePaper()),
                (makeLizard(), makeRock()),
                (makeLizard(), makeScissors()),
            };

            //Train the network
            Console.ReadLine();
            network.Reset();
            network.Train(new HyperParameters { MiniBatchSize = 10 }, data, null);

            Console.WriteLine("\nAfter training");
            Console.WriteLine("==============\n");

            testRockPaperScissorsLizardSpock(network);

            Console.WriteLine();
        }

        static string convertToName(Vector<double> result)
        {
            return Names[result.MaximumIndex()];
        }
        static string toString(Vector<double> vector, int length)
        {
            if (length == 3)
                return string.Format("({0:0.00}, {1:0.00}, {2:0.00})", vector[0], vector[1], vector[2]);
            else
                return string.Format("({0:0.00}, {1:0.00}, {2:0.00}, {3:0.00}, {4:0.00})", vector[0], vector[1], vector[2], vector[3], vector[4]);
        }
        static  void testRockPaperScissors(Network network, int length = 3)
        {
            var result = network.Detect(makeRock());
            Console.WriteLine(string.Format("{0,10} ---> {1,-10} {2}" , "Rock", convertToName(result), toString(result, length)));
            result = network.Detect(makePaper());
            Console.WriteLine(string.Format("{0,10} ---> {1,-10} {2}", "Paper", convertToName(result), toString(result, length)));
            result = network.Detect(makeScissors());
            Console.WriteLine(string.Format("{0,10} ---> {1,-10} {2}", "Scissors", convertToName(result), toString(result, length)));
        }
        static void testRockPaperScissorsLizardSpock(Network network)
        {
            testRockPaperScissors(network, 5);
            var result = network.Detect(makeLizard());
            Console.WriteLine(string.Format("{0,10} ---> {1,-10} {2}", "Lizard", convertToName(result), toString(result, 5)));
            result = network.Detect(makeSpock());
            Console.WriteLine(string.Format("{0,10} ---> {1,-10} {2}", "Spock", convertToName(result), toString(result, 5)));
        }
        static Vector<double> makeRock()
        {
           return CreateVector.DenseOfArray<double>(new double[] { 1, 0, 0, 0, 0 });
        }
        static Vector<double> makePaper()
        {
            return CreateVector.DenseOfArray<double>(new double[] { 0, 1, 0, 0, 0});
        }
        static Vector<double> makeScissors()
        {
           return CreateVector.DenseOfArray<double>(new double[] { 0, 0, 1, 0, 0});
        }
        static Vector<double> makeLizard()
        {
            return CreateVector.DenseOfArray<double>(new double[] { 0, 0, 0, 1, 0 });
        }
        static Vector<double> makeSpock()
        {
            return CreateVector.DenseOfArray<double>(new double[] { 0, 0, 0, 0, 1 });
        }
    }
}
