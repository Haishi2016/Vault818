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
        static void Main(string[] args)
        {
            Console.WriteLine("\nPlaying Rock Paper Scissors\n");
            PlayRockPaperScissors();

            Console.WriteLine("\nPlaying Rock Paper Scissors Lizard Spock\n");
            PlayRockPaperScissorsLizardSpock();
            Console.WriteLine();
        }

        private static void PlayRockPaperScissorsLizardSpock()
        {
            //A network with 3 input nodes representing rock, paper, scissors, respectively.
            //The network has 3 hidden nodes and 3 output nodes.
            Network network = new Network(new HyperParameters
            {
                CostFunctionName = "CrossEntropyCost",
                Epochs = 500, //try 20000
                MiniBatchSize = 10,
                LearningRate = 1,
                TestSize = 10
            }, 5, 5, 5);

            //Our training set is very small - basically it encodes the rock-paper-scissors rules: if paper, then scissors and so on.
            List<(Vector<double>, Vector<double>)> data = new List<(Vector<double>, Vector<double>)>
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

            //Before Training
            Console.WriteLine("Before training");
            Console.WriteLine("===============\n");
            test(network);

            Console.WriteLine();

            //Train the network
            network.Train(data, null);

            Console.WriteLine("After training");
            Console.WriteLine("==============\n");
            test(network);
        }

        private static void PlayRockPaperScissors()
        {
            //A network with 3 input nodes representing rock, paper, scissors, respectively.
            //The network has 3 hidden nodes and 3 output nodes.
            Network network = new Network(new HyperParameters
            {
                CostFunctionName = "CrossEntropyCost",
                Epochs = 100, //try 500, 20000
                MiniBatchSize = 3,
                LearningRate = 1,
                TestSize = 3
            }, 3, 3, 3);

            //Our training set is very small - basically it encodes the rock-paper-scissors rules: if paper, then scissors and so on.
            List<(Vector<double>, Vector<double>)> data = new List<(Vector<double>, Vector<double>)>
            {
                //rock, paper, scissors
                (makeRock(3), makePaper(3)),
                (makePaper(3), makeScissors(3)),
                (makeScissors(3), makeRock(3)),
            };

            //Before Training
            Console.WriteLine("Before training");
            Console.WriteLine("===============\n");
            test(network, 3);

            Console.WriteLine();

            //Train the network
            network.Train(data, null);

            Console.WriteLine("After training");
            Console.WriteLine("==============\n");
            test(network, 3);
        }

        static string convertToName(Vector<double> result)
        {
            if (result.Count == 3)
            {
                if (result[0] >= result[1] && result[0] >= result[2])
                    return "Rock    ";
                else if (result[2] >= result[1] && result[0] <= result[2])
                    return "Scissors";
                else
                    return "Paper   ";
            }
            else
            {
                if (result[0] >= result[1] && result[0] >= result[2] && result[0] >= result[3] && result[0] >= result[4])
                    return "Rock    ";
                else if (result[1] >= result[0] && result[1] >= result[2] && result[1] >= result[3] && result[1] >= result[4])
                    return "Paper   ";
                else if (result[2] >= result[0] && result[2] >= result[1] && result[2] >= result[3] && result[2] >= result[4])
                    return "Scissors";
                else if (result[3] >= result[0] && result[3] >= result[1] && result[3] >= result[2] && result[3] >= result[4])
                    return "Lizard  ";
                else
                    return "Spock   ";
            }
        }
        static string toString(Vector<double> vector)
        {
            if (vector.Count == 3)
                return string.Format("({0:0.000},{1:0.000},{2:0.000})", vector[0], vector[1], vector[2]);
            else
                return string.Format("({0:0.000},{1:0.000},{2:0.000}, {3:0.000}, {4:0.000})", vector[0], vector[1], vector[2], vector[3], vector[4]);
        }
        static  void test(Network network, int length=5)
        {
            //Test
            var result = network.Detect(makeRock(length));
            Console.WriteLine("Rock     --->  " + convertToName(result) + " " + toString(result));
            result = network.Detect(makePaper(length));
            Console.WriteLine("Paper    --->  " + convertToName(result) + " " + toString(result));
            result = network.Detect(makeScissors(length));
            Console.WriteLine("Scissors --->  " + convertToName(result) + " " + toString(result));
            if (length == 5)
            {
                result = network.Detect(makeSpock());
                Console.WriteLine("Spock    --->  " + convertToName(result) + " " + toString(result));
                result = network.Detect(makeLizard());
                Console.WriteLine("Lizard   --->  " + convertToName(result) + " " + toString(result));
            }
        }
        static Vector<double> makeRock(int length = 5)
        {
            if (length == 3)
                return CreateVector.DenseOfArray<double>(new double[] { 1, 0, 0 });
            else
                return CreateVector.DenseOfArray<double>(new double[] { 1, 0, 0, 0, 0 });
        }
        static Vector<double> makePaper(int length = 5)
        {
            if (length == 3)
                return CreateVector.DenseOfArray<double>(new double[] { 0, 1, 0 });
            else
                return CreateVector.DenseOfArray<double>(new double[] { 0, 1, 0, 0, 0});
        }
        static Vector<double> makeScissors(int length = 5)
        {
            if (length == 3)
                return CreateVector.DenseOfArray<double>(new double[] { 0, 0, 1 });
            else
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
