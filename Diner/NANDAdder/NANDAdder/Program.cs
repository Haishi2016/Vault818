using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NANDAdder
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("\nTesting NAND Gate...\n");
            Console.WriteLine(String.Format("TRUE NAND TRUE   = {0}", NAND.Eval(true, true)));
            Console.WriteLine(String.Format("TRUE NAND FALSE  = {0}", NAND.Eval(true, false)));
            Console.WriteLine(String.Format("FALSE NAND TRUE  = {0}", NAND.Eval(false, true)));
            Console.WriteLine(String.Format("FALSE NAND FALSE = {0}", NAND.Eval(false, false)));

            Console.WriteLine("\nTesting Bit Adder...\n");
            Console.WriteLine(bitAdd(0, 0));
            Console.WriteLine(bitAdd(0, 1));
            Console.WriteLine(bitAdd(1, 0));
            Console.WriteLine(bitAdd(1, 1));

            Console.WriteLine("\nTesting Byte Adder. Enter A and B to add them. Enter 0 for both inputs to exit.\n");
            while (true)
            {
                Console.Write("\n A = ");
                byte a = 0;
                byte b = 0;
                while (!byte.TryParse(Console.ReadLine(), out a))
                {
                    Console.WriteLine("Please enter a valid byte.");
                    Console.Write(" A = ");
                }
                Console.Write(" B = ");
                while (!byte.TryParse(Console.ReadLine(), out b))
                {
                    Console.WriteLine("Please enter a valid byte.");
                    Console.Write(" B = ");
                }
                if (a == 0 && b == 0)
                    break;
                var ret = ByteAdder.Add(a, b);
                Console.WriteLine(string.Format("\nResult={0}\n", ret.carry != 0? byte.MaxValue + 1 + ret.sum: ret.sum));
            }
        }
        static string bitAdd(byte a, byte b)
        {
            var ret = BitAdder.Add(a, b);
            return string.Format("{0} + {1} = {2}", a, b, (ret.carry > 0 ? "1" : "") + ret.sum);
        }
    }
}
