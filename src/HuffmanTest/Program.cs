using BenchmarkDotNet.Running;
using System;
using System.IO;
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

            HuffmanTester.RunArrayTests();

            //DumpDecodingArray();

            //var bytes = new byte[] { 255 };
            //var dst = new byte[10];
            //var result = Huffman.Decode(bytes, 0, 1, dst);

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