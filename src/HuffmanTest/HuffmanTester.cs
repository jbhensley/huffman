using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace HuffmanTest
{
    static class HuffmanTester
    {
        public static void RunArrayTests()
        {
            HuffmanArray.VerifyDecodingArray();
            CheckHuffmanHeader();
            RandomTest();
        }

        private static void CheckHuffmanHeader()
        {
            new HuffmanBench().Setup();
            foreach (var entry in HuffmanBench.s_headerData)
            {
                var decoded = new byte[entry.decodedValue.Length];
                HuffmanArray.Decode(entry.encoded, 0, entry.encoded.Length, decoded);

                Assert.Equal(entry.decodedValue, Encoding.ASCII.GetString(decoded));
            }
        }

        private static void RandomTest()
        {
            var random = System.Security.Cryptography.RandomNumberGenerator.Create();
            var lenByte = new byte[1];

            for(int i =0;i < 1_000_000;i++)
            {
                random.GetBytes(lenByte);
                var randBytes = new byte[lenByte[0]];
                random.GetBytes(randBytes);
                var encodedBytes = GetHuffmanEncodedBytes(randBytes);
                var unencodedBytes = new byte[lenByte[0]];
                var decodedBytes = HuffmanArray.Decode(encodedBytes, 0, encodedBytes.Length, unencodedBytes);

                Assert.True(Enumerable.SequenceEqual(randBytes, unencodedBytes));
                if (i % 10000 == 0)
                    Console.WriteLine($"Validated {i} random values");
            }
        }

        private static byte[] GetHuffmanEncodedBytes(byte[] value)
        {
            var encodedBytes = new List<byte>();
            byte workingByte = 0;
            int bitsLeftInByte = 8;
            foreach (var originalByte in value)
            {
                var encoded = HuffmanArray.Encode(originalByte);
                
                while (encoded.bitLength > 0)
                {
                    int bitsToWrite = bitsLeftInByte;
                    workingByte |= (byte)(encoded.encoded >> 24 + (8 - bitsToWrite));
                    if (encoded.bitLength >= bitsLeftInByte)
                    {
                        encoded.encoded <<= bitsLeftInByte;

                        encodedBytes.Add(workingByte);
                        workingByte = 0;
                        bitsLeftInByte = 8;
                    }
                    else
                        bitsLeftInByte -= encoded.bitLength;

                    encoded.bitLength -= bitsToWrite;
                }
            }

            if (bitsLeftInByte < 8)
            {
                // pad remaning bits with 1s
                workingByte |= (byte)((0x1 << bitsLeftInByte) - 1);
                encodedBytes.Add(workingByte);
            }

            return encodedBytes.ToArray();
        }
    }
}
