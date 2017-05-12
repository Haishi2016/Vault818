using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NANDAdder
{
    /// <summary>
    /// This is a byte adder implemented using bit adders
    /// </summary>
    public class ByteAdder
    {
        public static (byte sum, byte carry) Add(byte a, byte b)
        {
            byte result = 0;
            byte carry = 0;
            for (int i = 0; i < 8; i++)
            {
                var bitsum = BitAdder.Add((byte)(a >> i & 1), (byte)(b >> i & 1));
                var adjBitsum = BitAdder.Add(bitsum.sum, carry);
                carry = BitAdder.Add(bitsum.carry, adjBitsum.carry).sum;
                result = (byte)(result | (adjBitsum.sum << i));
            }
            return (result, carry);
        }
    }
}
