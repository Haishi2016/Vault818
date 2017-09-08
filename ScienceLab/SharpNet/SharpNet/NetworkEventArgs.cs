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
}
