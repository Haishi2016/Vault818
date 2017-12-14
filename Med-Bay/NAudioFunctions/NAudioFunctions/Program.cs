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
            SplitWaveFile(inputFile, Path.GetFileNameWithoutExtension(inputFile) + "{0}.wav", 1);
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
        static void SplitWaveFile(string file, string outputFilePattern, int chunkSizeinSeconds)
        {
            using (WaveFileReader reader = new WaveFileReader(file))
            {
                int bufferSize = reader.WaveFormat.AverageBytesPerSecond;
                byte[] buffer = new byte[bufferSize];
                int bytesRead = 0;
                int fileCount = 1;
                string fileName = string.Format(outputFilePattern, fileCount);
                WaveFileWriter writer = null;
                while ((bytesRead = reader.Read(buffer,0, buffer.Length)) > 0)
                {
                    if (writer == null)
                        writer = new WaveFileWriter(string.Format(outputFilePattern, fileCount), reader.WaveFormat);
                    writer.Write(buffer, 0, bytesRead);
                    if (reader.Position >= reader.Length - 1 || reader.Position >= chunkSizeinSeconds * bufferSize * fileCount)
                    {
                        writer.Close();
                        writer.Dispose();
                        writer = null;
                        fileCount++;
                    }
                }
            }
        }
    }
}
