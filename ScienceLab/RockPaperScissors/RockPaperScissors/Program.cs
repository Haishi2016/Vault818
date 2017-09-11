using MathNet.Numerics.LinearAlgebra;
using SharpNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
                Epochs = 290000, //try 20000
                MiniBatchSize = 3,
                LearningRate = 0.1,
                UseDropouts = true
            }, 5, 10, 5);

            network.OnSampleProcessed += Network_OnSampleProcessed;

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
            setupObservation(network);
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
            setupObservation(network);
            network.Reset();
            network.Train(new HyperParameters { MiniBatchSize = 10 }, data, null);

            Console.WriteLine("\nAfter training");
            Console.WriteLine("==============\n");

            testRockPaperScissorsLizardSpock(network);

            Console.WriteLine();
        }

        private static void setupObservation(Network network)
        {
            Console.WriteLine("\nObserve Training?");
            Console.WriteLine("==================");
            Console.WriteLine("1....Observe samples");
            Console.WriteLine("2....Observe mini-batches");
            Console.WriteLine("0....No");
            Console.WriteLine("==================");
            Console.Write("]");
            var input = Console.ReadLine();
            int selection = 0;
            if (int.TryParse(input, out selection))
            {
                if (selection < 0)
                    selection = 0;
                else if (selection > 2)
                    selection = 2;
            }
            else
                selection = 0;
            switch (selection)
            {
                case 0:
                    network.OnSampleProcessed -= Network_OnSampleProcessed;
                    network.OnMiniBatchProcessed -= Network_OnSampleProcessed;
                    break;
                case 1:
                    network.OnSampleProcessed += Network_OnSampleProcessed;
                    Console.Clear();
                    break;
                case 2:
                    network.OnMiniBatchProcessed += Network_OnSampleProcessed;
                    Console.Clear();
                    break;
            }
        }

        private static void Network_OnSampleProcessed(object sender, NetworkInsightEventArgs e)
        {
            Console.CursorLeft = 0;
            Console.CursorTop = 0;
            Console.CursorVisible = false;
            for (int i = 0; i < e.WeightList.Count; i++)
            {
                Console.WriteLine("\nLayer " + i);
                Console.WriteLine("-------");
                
                if (e.WeightList[i] == null)
                {
                    Console.Write(" (");
                    for (int j = 0; j < e.BiasList[i].Count; j++)
                        Console.Write(string.Format("{0: 000.000;-000.000}{1} ", e.BiasList[i][j], j < e.BiasList[i].Count - 1 ? "," : ")\n"));
                }
                else
                {
                    Console.Write(" ");
                    for (int j = 0; j < e.WeightList[i].RowCount; j++)
                    {
                        Console.Write(string.Format("{0: 000.000;-000.000} ", e.BiasList[i][j]));
                        Console.Write("|");
                        for (int k = 0; k < e.WeightList[i].ColumnCount; k++)
                            Console.Write(string.Format("{0: 000.000;-000.000}{1} ", e.WeightList[i][j,k], k < e.WeightList[i].ColumnCount - 1 ? "," : "|\n"));
                    }
                }
            }
            Console.CursorVisible = true;
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
            Console.WriteLine(string.Format("{0,32}  {1} {2} {3}  {4}", "Rock", "Paper", "Scis.", (length != 3? "Liz.": ""), (length != 3? "Spock" : "")));
            Console.WriteLine(string.Format("{0,32}  {1} {2} {3} {4}", "-----", "-----", "-----", (length != 3? "-----": ""), (length != 3? "-----": "")));
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
