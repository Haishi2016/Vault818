using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpNet
{
    public class HyperParameters
    {
        public bool UseDropouts { get; set; }
        public double HiddenLayerDropoutProbability { get; set; }
        public double InputLayerDropoutProbability { get; set; }
        public double LearningRate { get; set; }
        public int MiniBatchSize { get; set; }
        public bool AutoSave { get; set; }
        public double AutoSaveThreshold { get; set; }
        public double RegulationLambda { get; set; }
        public string CostFunctionName { get; set; }
        public int Epochs { get; set; }
        public int TestSize { get; set; }
       
        public string Serialize()
        {
            return JsonConvert.SerializeObject(this, Formatting.None);
        }
        public static HyperParameters Deserialize(string val)
        {
            return JsonConvert.DeserializeObject<HyperParameters>(val);
        }
    }
}
