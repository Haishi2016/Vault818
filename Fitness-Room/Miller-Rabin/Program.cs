using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace MillerRabin
{
    class Program
    {
        static Random rand = new Random();
        static void Main(string[] args)
        {
            int count = 1000000;
            BigInteger min = 1;
            bool[] result = new bool[count];
            Stopwatch watch = new Stopwatch();
            watch.Start();
            for (int i = 0; i < count; i++)
                result[i] = isPrime(min + i);
            watch.Stop();
            int primeCount = 0;
            for (int i = 0; i < count; i++)
            {
                if (result[i])
                {
                    Console.Write(min + i + "\t");
                    primeCount++;
                }
            }
            Console.WriteLine("\n Time elapsed (ms): " + watch.ElapsedMilliseconds);
            Console.WriteLine("\n # of prime numbers: " + primeCount);
        }
        static bool isPrime(BigInteger number, int? iterations = 40)
        {
            if (number == 2 || number == 3)
                return true;
            if (number % 2 == 0 || number < 2)
                return false;
            int r = 0;
            BigInteger d = number - 1;
            while (d % 2 == 0)
            {
                d /= 2;
                r++;
            }
            for (int i =0; i < iterations; i++)
            {
                BigInteger a = randomBigInteger(2, number - 2);
                BigInteger x = BigInteger.ModPow(a, d, number);
                if (x == 1 || x == number - 1)
                    continue;
                for (int j = 0; j < r-1; j++)
                {
                    x = BigInteger.ModPow(x, 2, number);
                    if (x == 1)
                        return false;
                    if (x == number - 1)
                        break;                    
                }
                if (x != number - 1)
                    return false;
            }
            return true;
        }
        static BigInteger randomBigInteger(BigInteger min, BigInteger max)
        {
            RandomNumberGenerator gen = RandomNumberGenerator.Create();
            byte[] bytes = (max - min).ToByteArray();
            byte[] newBytes = new byte[bytes.Length];
            int top = bytes[bytes.Length - 1];
            for (int i = 0; i < bytes.Length; i++)
            {
                if (i == bytes.Length - 1)
                    newBytes[i] = (byte)rand.Next(0, top + 1);
                else
                    newBytes[i] = (byte)rand.Next(0, 256);                
            }
            var result = new BigInteger(newBytes);
            if (result >= max)
                result >>= 1;
            return result + min;
        }
    }
}
