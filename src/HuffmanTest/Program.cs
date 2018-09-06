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

            // Profiling
            //bench.Setup();
            //for (var i = 0; i < 10000; i++)
            //{
            //    bench.Array();
            //}

            //var summary = BenchmarkRunner.Run<HuffmanBench>();

            //bench.Setup();
            //for (var i = 0; i < 20_000; i++)
            //{
            //    bench.OrigOpt();
            //}

            //HuffmanArray.VerifyDecodingArray();
            //bench.Setup();
            //bench.Array();

            //CheckHuffman();

            HuffmanTester.RunArrayTests();
            

            //DumpDecodingArray();
            //VerifyDecodingArray();

            Console.WriteLine("Finished");
            Console.ReadLine();
        }
        
        static void CheckHuffman()
        {
            //var bench = new HuffmanBench();
            //bench.Setup();

            foreach (var entry in HuffmanBench.s_headerData)
            {
                var decoded = new byte[entry.decodedValue.Length];
                Huffman.Decode(entry.encoded, 0, entry.encoded.Length, decoded);

                Assert.Equal(entry.decodedValue, Encoding.ASCII.GetString(decoded));
            }
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

        static void DumpDecodingDictionary()
        {
            using (var writer = File.CreateText("c:\\temp\\out.txt"))
            {
                foreach (var entry in HuffmanDict.s_decodingDictionary)
                    writer.WriteLine($"{FormatBinaryString(entry.Key)},{entry.Value.BitLength},{entry.Value.DecodedValue},{FormatBinaryString(entry.Value.DecodedValue)}");
            }

            string FormatBinaryString(uint value)
            {
                string sval = Convert.ToString(value, 2);
                sval = new string('0', 32 - sval.Length) + sval;
                return sval.Insert(24, "_").Insert(16, "_").Insert(8, "_");
            }
        }
    }
}
