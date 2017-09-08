using MathNet.Numerics.LinearAlgebra;
using SharpNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicTacToe
{
    class Program
    {
        static void Main(string[] args)
        {
            //A network with 3 input nodes representing rock, paper, scissors, respectively.
            //The network has 3 hidden nodes and 3 output nodes.
            Network network = new Network(new HyperParameters
            {
                 CostFunctionName = "CrossEntropyCost",
                 Epochs = 100,
                 MiniBatchSize = 3,
                 LearningRate = 1,
                 TestSize = 3
            }, 3, 3, 3);

            //Our training set is very small - basically it encodes the rock-paper-scissors rules: if paper, then scissors and so on.
            List<(double[], Vector<double>)> data = new List<(double[], Vector<double>)>
            {
                //rock, paper, scissors
                (new double[]{1,0,0}, CreateVector.DenseOfArray<double>(new double[]{0,1,0})),
                (new double[]{0,1,0}, CreateVector.DenseOfArray<double>(new double[]{0,0,1})),
                (new double[]{0,0,1}, CreateVector.DenseOfArray<double>(new double[]{1,0,0})),
            };

            //Before Training
            Console.WriteLine("Before training");
            test(network);

            Console.WriteLine();

            //Train the network
            network.Train(data, null);

            Console.WriteLine("After training");
            test(network);
        }
        static string convertToName(Vector<double> result)
        {
            if (result[0] >= result[1] && result[0] >= result[2])
                return "Rocks   ";
            else if (result[2] >= result[1] && result[0] <= result[2])
                return "Scissors";
            else
                return "Paper   ";
        }
        static string toString(Vector<double> vector)
        {
            return string.Format("({0:0.000},{1:0.000},{2:0.000})", vector[0], vector[1], vector[2]);
        }
        static  void test(Network network)
        {
            //Test
            var result = network.Detect(makeRock());
            Console.WriteLine("Rock     --->  " + convertToName(result) + " " + toString(result));
            result = network.Detect(makePaper());
            Console.WriteLine("Paper    --->  " + convertToName(result) + " " + toString(result));
            result = network.Detect(makeScissors());
            Console.WriteLine("Scissors --->  " + convertToName(result) + " " + toString(result));
        }
        static Vector<double> makeRock()
        {
            return CreateVector.DenseOfArray<double>(new double[] { 1, 0, 0 });
        }
        static Vector<double> makePaper()
        {
            return CreateVector.DenseOfArray<double>(new double[] { 0, 1, 0 });
        }
        static Vector<double> makeScissors()
        {
            return CreateVector.DenseOfArray<double>(new double[] { 0, 0, 1 });
        }
    }
}
