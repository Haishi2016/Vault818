using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitClassifier
{
    public class MINSTDataLoader
    {
        /// <summary>
        /// [offset] [type]          [value]          [description] 
        ///0000     32 bit integer  0x00000803(2051) magic number 
        ///0004     32 bit integer  60000            number of images 
        ///0008     32 bit integer  28               number of rows 
        ///0012     32 bit integer  28               number of columns 
        ///0016     unsigned byte   ??               pixel 
        ///0017     unsigned byte   ??               pixel
        ///........
        ///xxxx unsigned byte   ??               pixel
        /// </summary>
        /// <param name="imageFile"></param>
        /// <returns></returns>
        public static List<byte[]> LoadImages(string imageFile)
        {
            if (!File.Exists(imageFile))
                throw new FileNotFoundException();
            List<byte[]> ret = new List<byte[]>();
            using (FileStream stream = File.OpenRead(imageFile))
            {
                using (GZipStream uncompressed = new GZipStream(stream, CompressionMode.Decompress))
                {
                    using (BinaryReader reader = new BinaryReader(uncompressed))
                    {
                        int magicNumber = toLittleEndian(reader.ReadInt32());
                        if (magicNumber != 0x803)
                            throw new InvalidDataException("0x803 is expected at offset 000");
                        int imageCount = toLittleEndian(reader.ReadInt32());
                        int rows = toLittleEndian(reader.ReadInt32());
                        int colums = toLittleEndian(reader.ReadInt32());
                        for (int i = 0; i < imageCount; i++)
                            ret.Add(reader.ReadBytes(rows * colums));
                    }
                }
            }
            return ret;
        }
        private static Int32 toLittleEndian(Int32 bigEndian)
        {
            var bytes = BitConverter.GetBytes(bigEndian);
            Array.Reverse(bytes);
            return BitConverter.ToInt32(bytes,0);
        }
        /// <summary>
        /// [offset] [type]          [value]          [description] 
        ///0000     32 bit integer  0x00000801(2049) magic number(MSB first)
        ///0004     32 bit integer  60000            number of items 
        ///0008     unsigned byte   ??               label 
        ///0009     unsigned byte   ??               label
        ///........
        ///xxxx unsigned byte   ??               label
        ///The labels values are 0 to 9. 
        /// </summary>
        /// <param name="labelFile"></param>
        /// <returns></returns>
        public static List<byte> LoadLabels(string labelFile)
        {
            if (!File.Exists(labelFile))
                throw new FileNotFoundException();
            List<byte> ret = new List<byte>();
            using (FileStream stream = File.OpenRead(labelFile))
            {
                using (GZipStream uncompressed = new GZipStream(stream, CompressionMode.Decompress))
                {
                    using (BinaryReader reader = new BinaryReader(uncompressed))
                    {
                        int magicNumber = toLittleEndian(reader.ReadInt32());
                        if (magicNumber != 0x801)
                            throw new InvalidDataException("0x801 is expected at offset 000");
                        int labelCount = toLittleEndian(reader.ReadInt32());
                        ret.AddRange(reader.ReadBytes(labelCount));
                    }
                }
            }
            return ret;
        }
        public static List<(byte[] Image,byte Label)> CombineImagesAndLabels(List<byte[]> images, List<byte> labels)
        {
            if (images == null | labels == null)
                throw new ArgumentNullException();
            if (images.Count != labels.Count)
                throw new ArgumentException("List lengths don't match.");
            List<(byte[], byte)> ret = new List<(byte[], byte)>();
            for (int i = 0; i < images.Count; i++)
                ret.Add((images[i], labels[i]));
            return ret;
        }

        public static double[] Normalize(byte[] data)
        {
            double[] ret = new double[data.Length];
            for (int i = 0; i < ret.Length; i++)
                ret[i] = data[i] / 255.0;
            return ret;
        }
    }
}
