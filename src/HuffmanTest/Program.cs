using BenchmarkDotNet.Running;
using System;
using System.IO;
using System.Linq;
using System.Text;
using Xunit;

namespace HuffmanTest
{
    class Program
    {
        static void Main(string[] args)
        {
            //var bench = new HuffmanBench();
            //var summary = BenchmarkRunner.Run<HuffmanBench>();

            //HuffmanTester.RunArrayTests();

            //DumpDecodingArray();


            var source = new byte[6];
            var destination = new byte[24];

            for (int i = 0; i < 255; i++)
            {
                ulong currentBits = 0;  // We can have 7 bits of rollover plus 30 bits for the next encoded value, so use a ulong
                int currentBitCount = 0;
                int dstOffset = 0;

                ulong code = 0;
                int bitLength = 0;
                for (int j = 0; j < 3; j++)
                {
                    if (j == 1)
                    {
                        // hard-coded EOS on the second pass
                        code = (ulong)(0b11111111_11111111_11111111_11111100) << 32;
                        bitLength = 30;
                    }

                    else
                        (code, bitLength) = HuffmanDecodingTests.GetEncodedValue(source[0]);

                    currentBits |= code >> currentBitCount;
                    currentBitCount += bitLength;

                    while (currentBitCount >= 8)
                    {
                        destination[dstOffset++] = (byte)(currentBits >> 56);
                        currentBits = currentBits << 8;
                        currentBitCount -= 8;
                    }
                }

                // Fill any trailing bits with ones, per RFC
                if (currentBitCount > 0)
                {
                    currentBits |= 0xFFFFFFFFFFFFFFFF >> currentBitCount;
                    destination[dstOffset++] = (byte)(currentBits >> 56);
                }

                //yield return new object[] { destination.Take(dstOffset).ToArray() };
                var dst = new byte[24];
                //Assert.Throws<HuffmanDecodingException>(() => Huffman.Decode(destination.Take(dstOffset).ToArray(), dst));
            }

            Console.WriteLine("Finished");
            Console.ReadLine();
        }

        static void DumpDecodingArray()
        {
            using (var writer = File.CreateText("c:\\temp\\out.txt"))
            {
                var length0 = Huffman.s_decodingArray.GetLength(0);
                var length1 = Huffman.s_decodingArray.GetLength(1);

                for (int i = 0; i < length0; i++)
                {
                    writer.Write("{");
                    for (int j = 0; j < length1; j++)
                    {
                        writer.Write(Huffman.s_decodingArray[i, j]);
                        if (j < length1 - 1)
                            writer.Write(",");
                    }
                    writer.WriteLine("}");
                }
            }
        }
    }
}