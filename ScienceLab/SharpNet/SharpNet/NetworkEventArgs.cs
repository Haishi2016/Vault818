using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpNet
{
    public class TrainingEventArgs : EventArgs
    {
        public int Epoch { get; private set; }
        public int Sample { get; private set; }
        public int BatchSize { get; private set; }
        public int DataSize { get; private set; }
        public int TestSize { get; private set; }
        public double DetectionRate { get; private set; }
        public TrainingEventArgs(int epoch = -1, int sample = -1, int batchSize = -1, int dataSize = -1, int testSize = -1, double detectionRate = -1.0)
        {
            Epoch = epoch;
            Sample = sample;
            BatchSize = batchSize;
            DataSize = dataSize;
            TestSize = testSize;
            DetectionRate = detectionRate;
        }
    }
    public class NetworkInsightEventArgs: EventArgs
    {
        public List<Vector<double>> BiasList { get; private set; }
        public List<Matrix<double>> WeightList { get; private set; }
        public NetworkInsightEventArgs(List<Vector<double>> biasList, List<Matrix<double>> weightList)
        {
            BiasList = new List<Vector<double>>();
            for (int i = 0; i < biasList.Count; i++)
                BiasList.Add(biasList[i].Clone());
            WeightList = new List<Matrix<double>>();
            for (int i = 0; i < weightList.Count; i++)
            {
                if (weightList[i] != null)
                    WeightList.Add(weightList[i].Clone());
                else
                    WeightList.Add(null);
            }
        }
    }
}
