using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NAudioFunctions
{
    class Program
    {
        static void Main(string[] args)
        {
            string inputFile = "exciting.wav";
            string outputFile = Path.GetFileNameWithoutExtension(inputFile) + "_mono.wav";
            ConvertWaveToMonoWave(inputFile, outputFile);
        }
        static void ConvertMP3toWave(string file, string outputFile)
        {
            using (Mp3FileReader reader = new Mp3FileReader(file))
            {
                WaveFileWriter.CreateWaveFile(outputFile, reader);
            }
        }
        static void ConvertWaveToMonoWave(string file, string outputFile)
        {
            using (WaveFileReader reader = new WaveFileReader(file))                
            {
                WaveFormat format = new WaveFormat(16000, 16, 1);            
                using (var conversionStream = WaveFormatConversionStream.CreatePcmStream(reader))
                {
                    using (var upSampler = new WaveFormatConversionStream(format, conversionStream))
                    {
                        WaveFileWriter.CreateWaveFile(outputFile, upSampler);
                    }
                }                
            }
        }
    }
}
