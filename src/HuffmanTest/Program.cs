using BenchmarkDotNet.Running;
using System;
using System.Collections.Generic;
using System.IO;
using Xunit;

namespace HuffmanTest
{
    class Program
    {
        static void Main(string[] args)
        {
            BenchmarkRunner.Run<HuffmanTests>();

            //var tests = new HuffmanTests();
            //tests.SetupPerfTest();
            //tests.ProcessHeadersWithLoop();
            //tests.ProcessHeadersWithLoopAndComputedCodeMax();
            //tests.ProcessHeadersWithDictionary();

            //Console.WriteLine("Finished");
            //Console.ReadLine();
        }

        static void ComputeCodeMax()
        {
            int codeMax = 0;
            var s_decodingTable = Huffman.s_decodingTable;
            for (int i = 0; i < s_decodingTable.Length; i++)
            {
                (int codeLength, int[] codes) = s_decodingTable[i];

                if (i > 0)
                {
                    codeMax <<= codeLength - s_decodingTable[i - 1].codeLength;
                }

                codeMax += codes.Length;

                Console.WriteLine($"{i}:\t{codeMax}");
            }
        }
    }
}
