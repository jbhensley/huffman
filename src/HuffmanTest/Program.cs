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


            //0b11111111_11111111_11111111_11111100 // EOS
            //var EOS = new byte[] { 0b11111111, 0b11111111, 0b11111111, 0b11111100 };
            // a character followed by EOS
            //var original = new byte[] { (byte)'a' }.Concat(EOS).ToArray();
            //var encoded = new byte[10];
            //var count = HuffmanDecodingTests.Encode(original, encoded);
            //var dst = new byte[10];
            //var result = Huffman.Decode(encoded, 0, count, dst);

            // send single EOS
            var EOS = new byte[] { 0b11111111, 0b11111111, 0b11111111, 0b11111100 };
            //var source = new byte[1];
            var destination = new byte[12];
            for (int i = 0; i < 256; i++)
            {
                var encoded = HuffmanDecodingTests.GetEncodedValue((byte)i);
                
                //source[0] = (byte)i;
                //int encodedByteCount = Encode(source, destination);
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