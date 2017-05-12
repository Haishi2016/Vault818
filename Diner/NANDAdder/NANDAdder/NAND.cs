using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NANDAdder
{
    /// <summary>
    /// This is a NAND gate implemented as a perceptron
    ///      -2
    /// a o-------+----+
    ///           |    |
    ///           |  3 |-----o r
    ///      -2   |    |
    /// b o-------+----+
    /// 
    ///     / a * -2 + b * -2 + 3 > 0       1 
    /// r = |
    ///     \ a * -2 + b * -2 + 3 < 0       0
    /// </summary>
    public class NAND
    {
        public static bool Eval(bool a, bool b)
        {
            return ((a ? 1 : 0) * -2 + (b ? 1 : 0) * -2 + 3) > 0;
        }
        public static byte Eval(byte a, byte b)
        {
            if (a != 0 && a != 1 || b != 0 && b != 1)
                throw new ArgumentException("Only 0 and 1 are allowed.");
            return (byte)(a * -2 + b * -2 + 3 > 0 ? 1 : 0);
        }
    }
}
