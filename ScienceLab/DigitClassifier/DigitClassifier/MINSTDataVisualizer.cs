using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitClassifier
{
    public class MINSTDataVisualizer
    {
        private const string GrayScale = "$@B%8&WM#*oahkbdpqwmZO0QLCJUYXzcvunxrjft/\\|()1{}[]?-_+~<>i!lI;:,\"^`\'. ";
        public static void PrintImage(double[] image, int row)
        {
            int column = image.Length / row;
            for (int y = 0; y < row; y++)
            {
                for (int x = 0; x < column; x++)
                {
                    Console.Write(GrayScale[(int)((GrayScale.Length-1) * (1-image[y * column + x]))]);
                }
                Console.WriteLine();
            }
        }
    }
}
