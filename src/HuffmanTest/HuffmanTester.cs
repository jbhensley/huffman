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
            VerifyDecodingArray();
            CheckHuffmanHeader();
            RandomTest();
        }

        public static void VerifyDecodingArray()
        {
            for (int i = 0; i < Huffman.s_encodingTable.Length - 1; i++)    // Length -1 because the last entry is the EOS symbol, which must not be used
            {
                (uint code, int bitLength) = Huffman.s_encodingTable[i];
                var bytes = new byte[(int)Math.Ceiling(bitLength / 8.0)];
                int index = 0;
                for (int j = 0; j < bitLength; j += 8)
                    bytes[index++] = (byte)((code >> (24 - j)) & 0xFF);

                var bitsLeftInByte = 8 - (bitLength % 8);
                if (bitsLeftInByte < 8)
                    bytes[bytes.Length - 1] |= (byte)((0x1 << bitsLeftInByte) - 1);
                //if ((bytes[bytes.Length - 1] & 0xFF) == 0xFF)
                //{
                //    var array = new byte[bytes.Length - 1];
                //    for (int j = 0; j < array.Length; j++)
                //        array[j] = bytes[j];

                //    bytes = array;
                //}
                var dst = new byte[1];
                int decoded = Huffman.Decode(bytes, 0, bytes.Length, dst);
                Assert.NotEqual(-1, decoded);
                Assert.Equal(1, decoded);
                //Assert.Equal(bitLength, decoded);
                Assert.Equal(i, dst[0]);
            }
        }

        private static void CheckHuffmanHeader()
        {
            new HuffmanBench().Setup();
            foreach (var entry in HuffmanBench.s_headerData)
            {
                var decoded = new byte[entry.decodedValue.Length];
                Huffman.Decode(entry.encoded, 0, entry.encoded.Length, decoded);

                Assert.Equal(entry.decodedValue, Encoding.ASCII.GetString(decoded));
            }
        }

        private static void RandomTest()
        {
            var random = System.Security.Cryptography.RandomNumberGenerator.Create();
            var lenByte = new byte[1];

            for (int i = 0; i < 1_000_000; i++)
            {
                random.GetBytes(lenByte);
                var randBytes = new byte[lenByte[0]];
                random.GetBytes(randBytes);
                var encodedBytes = GetHuffmanEncodedBytes(randBytes);
                var unencodedBytes = new byte[lenByte[0]];
                var decodedBytes = Huffman.Decode(encodedBytes, 0, encodedBytes.Length, unencodedBytes);

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
                var encoded = Huffman.Encode(originalByte);
                
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
