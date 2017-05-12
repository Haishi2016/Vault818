using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NANDAdder
{
    /// <summary>
    /// This is a bit adder implemented using NAND gates
    /// </summary>
    public class BitAdder
    {
        public static (byte sum, byte carry) Add(byte a, byte b)
        {
            if (a != 0 && a != 1 || b != 0 && b != 1)
                throw new ArgumentException("Only 0 and 1 are allowed.");
            byte intValue = NAND.Eval(a, b);

            return (NAND.Eval(NAND.Eval(a, intValue), NAND.Eval(intValue, b)), NAND.Eval(intValue, intValue));
        }
    }
}
