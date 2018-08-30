using BenchmarkDotNet.Attributes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace HuffmanTest
{
    [MemoryDiagnoser]
    [InvocationCount(10000)]
    public class HuffmanTests
    {
        public static List<(byte[] encodedValue, string decodedValue)> s_HeaderList = new List<(byte[] encodedValue, string decodedValue)>();

        [GlobalSetup]
        public void SetupPerfTest()
        {
            // build the dictionary
            var dict = BuildHuffmanDictionary();
            VerifyHuffmanDictionary(dict);
            Huffman.s_decodingDictionary = dict;

            // prepare data to decode
            using (var reader = File.OpenText(@".\headers.txt"))    // file must be set to copy to output dir for this to work
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                    s_HeaderList.Add((GetHuffmanEncodedBytes(line), line));
            }
        }

        [Benchmark(Baseline = true)]
        public void ProcessHeadersWithLoop()
        {
            ProcessHeaders(Huffman.DecodingMethod.Loop);
        }

        [Benchmark]
        public void ProcessHeadersWithLoopAndComputedCodeMax()
        {
            ProcessHeaders(Huffman.DecodingMethod.LoopWithCodeMax);
        }

        [Benchmark]
        public void ProcessHeadersWithDictionary()
        {
            ProcessHeaders(Huffman.DecodingMethod.Dictionary);
        }

        void ProcessHeaders(Huffman.DecodingMethod decodingMethod)
        {
            foreach (var entry in s_HeaderList)
            {
                var decodedBytes = new byte[entry.decodedValue.Length];
                var decoded = Huffman.Decode(entry.encodedValue, 0, entry.encodedValue.Length, decodedBytes, decodingMethod);
                if (Encoding.ASCII.GetString(decodedBytes) != entry.decodedValue)
                    throw new Exception("Decoding error");
            }
        }

        #region Helper Methods
        public static Dictionary<uint, Huffman.DecodingTableEntry> BuildHuffmanDictionary()
        {
            var dict = new Dictionary<uint, Huffman.DecodingTableEntry>();

            // load all entries from the encoding table. we'll remove items as they are hashed
            var nonHashedEntries = new List<((uint code, int bitLength) tableEntry, uint virtualDictionaryMask, uint decodedValue)>();
            for (uint i = 0; i < Huffman.s_encodingTable.Length; i++)
                nonHashedEntries.Add((Huffman.s_encodingTable[i], 0, i));

            int workingByte = 3;
            int maxBitLength = 0;
            uint nextLevel = 0;
            do
            {
                nextLevel += 0x01_00_00_00; // level is indicated the most significant byte
                maxBitLength += 8;  // the max number of bits that can be used for encoding at the current level
                uint codeMask = (uint)0xFF << (8 * workingByte);    // mask to extract the working byte from "code"

                for (int i = nonHashedEntries.Count - 1; i >= 0; i--)   // start at the end and loop backward so we can remove entries as we go
                {
                    var entry = nonHashedEntries[i];
                    var tableEntry = entry.tableEntry;

                    uint codeByte = (tableEntry.code & codeMask) >> (8 * workingByte);  // extract the working byte and shift it all the way to the right
                    uint key = entry.virtualDictionaryMask | codeByte;                  // key includes virtual dictionary mask and working byte value

                    // if the code's bit length <= maxBitLength then add code with all permutations of bits that are beyond the length of the code
                    if (tableEntry.bitLength <= maxBitLength)
                    {
                        int loopMax = 0x1 << (maxBitLength - tableEntry.bitLength); // have to create entries for all of these values
                        for (uint j = 0; j < loopMax; j++)
                            dict.Add(key + j, new Huffman.DecodingTableEntry(entry.decodedValue, tableEntry.bitLength));    // each entry decodes to the same value

                        nonHashedEntries.RemoveAt(i);
                    }
                    else
                    {

                        // the next virtual dictionary is identified by the level and current working byte pattern
                        uint nextVirtualDictionaryMask =
                            nextLevel                        // next level goes in most significant byte
                            | (codeByte << 16);              // put the current byte pattern in the second most significant byte

                        nonHashedEntries[i] = (tableEntry, nextVirtualDictionaryMask, entry.decodedValue);

                        // create an entry that points to the next virtual dictionary. a bit length of zero will be used to identify these
                        if (!dict.ContainsKey(key))
                            dict.Add(key, new Huffman.DecodingTableEntry(nextVirtualDictionaryMask, 0));
                    }
                }

                workingByte--;

            } while (nonHashedEntries.Count > 0);

            return dict;
        }

        public static void VerifyHuffmanDictionary(Dictionary<uint, Huffman.DecodingTableEntry> dictionary)
        {
            for (uint i = 0; i < Huffman.s_encodingTable.Length; i++)
            {
                (uint code, int bitLength) = Huffman.s_encodingTable[i];
                Huffman.DecodingTableEntry entry;

                int byteNumber = 3;                     // grab the most significant byte
                uint virtualDictionaryMask = 0;         // used to key into different "virtual" dictionaries contained in our single dictionary
                do
                {
                    // extract the working byte
                    uint workingByte = code >> (byteNumber * 8) & 0xFF;

                    // apply virtual dictionary bitmask
                    workingByte |= virtualDictionaryMask;

                    // key into the dictionary
                    Assert.True(dictionary.TryGetValue(workingByte, out entry));

                    // if we get a length back then we have found the decoded value
                    if (entry.BitLength > 0)
                    {
                        Assert.Equal(entry.DecodedValue, i);
                        Assert.Equal(entry.BitLength, bitLength);
                    }
                    // otherwise, we have found a mask that lets us key into the next virtual dictionary
                    else
                    {
                        virtualDictionaryMask = entry.DecodedValue;
                        byteNumber--;
                    }

                } while (entry.BitLength == 0);
            }
        }

        public static byte[] GetHuffmanEncodedBytes(string value)
        {
            var encodedBytes = new List<byte>();
            byte workingByte = 0;
            int bitsLeftInByte = 8;
            foreach (var character in value)
            {
                var encoded = Huffman.Encode(character);
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
        #endregion
    }
}
