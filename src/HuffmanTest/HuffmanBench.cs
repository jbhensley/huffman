// Derived from:
// https://raw.githubusercontent.com/aspnet/KestrelHttpServer/64127e6c766b221cf147383c16079d3b7aad2ded/test/Kestrel.Core.Tests/HuffmanTests.cs

// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using BenchmarkDotNet.Attributes;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Xunit;

namespace HuffmanTest
{
    [MemoryDiagnoser]
    public class HuffmanBench
    {
        private const int _iterations = 1000;

        private const int _simpleCount = 26;
        private static readonly (byte[] encoded, byte[] expected)[] s_simpleData = new (byte[], byte[])[_simpleCount]
        {
            // Single 5-bit symbol
            ( new byte[] { 0x07 }, Encoding.ASCII.GetBytes("0") ),
            // Single 6-bit symbol
            ( new byte[] { 0x57 }, Encoding.ASCII.GetBytes("%") ),
            // Single 7-bit symbol
            ( new byte[] { 0xb9 }, Encoding.ASCII.GetBytes(":") ),
            // Single 8-bit symbol
            ( new byte[] { 0xf8 }, Encoding.ASCII.GetBytes("&") ),
            // Single 10-bit symbol
            ( new byte[] { 0xfe, 0x3f }, Encoding.ASCII.GetBytes("!") ),
            // Single 11-bit symbol
            ( new byte[] { 0xff, 0x7f }, Encoding.ASCII.GetBytes("+") ),
            // Single 12-bit symbol
            ( new byte[] { 0xff, 0xaf }, Encoding.ASCII.GetBytes("#") ),
            // Single 13-bit symbol
            ( new byte[] { 0xff, 0xcf }, Encoding.ASCII.GetBytes("$") ),
            // Single 14-bit symbol
            ( new byte[] { 0xff, 0xf3 }, Encoding.ASCII.GetBytes("^") ),
            // Single 15-bit symbol
            ( new byte[] { 0xff, 0xf9 }, Encoding.ASCII.GetBytes("<") ),
            // Single 19-bit symbol
            ( new byte[] { 0xff, 0xfe, 0x1f }, Encoding.ASCII.GetBytes("\\") ),
            // Single 20-bit symbol
            ( new byte[] { 0xff, 0xfe, 0x6f }, new byte[] { 0x80 } ),
            // Single 21-bit symbol
            ( new byte[] { 0xff, 0xfe, 0xe7 }, new byte[] { 0x99 } ),
            // Single 22-bit symbol
            ( new byte[] { 0xff, 0xff, 0x4b }, new byte[] { 0x81 } ),
            // Single 23-bit symbol
            ( new byte[] { 0xff, 0xff, 0xb1 }, new byte[] { 0x01 } ),
            // Single 24-bit symbol
            ( new byte[] { 0xff, 0xff, 0xea }, new byte[] { 0x09 } ),
            // Single 25-bit symbol
            ( new byte[] { 0xff, 0xff, 0xf6, 0x7f }, new byte[] { 0xc7 } ),
            // Single 26-bit symbol
            ( new byte[] { 0xff, 0xff, 0xf8, 0x3f }, new byte[] { 0xc0 } ),
            // Single 27-bit symbol
            ( new byte[] { 0xff, 0xff, 0xfb, 0xdf }, new byte[] { 0xcb } ),
            // Single 28-bit symbol
            ( new byte[] { 0xff, 0xff, 0xfe, 0x2f }, new byte[] { 0x02 } ),
            // Single 30-bit symbol
            ( new byte[] { 0xff, 0xff, 0xff, 0xf3 }, new byte[] { 0x0a } ),

            //               h      e         l          l      o         *
            ( new byte[] { 0b100111_00, 0b101_10100, 0b0_101000_0, 0b0111_1111 }, Encoding.ASCII.GetBytes("hello") ),

            //               h      e         l          l      o         <space>    W           O          R            L            D     \                              >                *
            ( new byte[] { 0b100111_00, 0b101_10100, 0b0_101000_0, 0b0111_0101, 0b00_111001, 0b0_1101010, 0b1101101_1, 0b100111_10, 0b11111_111, 0b11111111, 0b11110000, 0b11111111, 0b1011_1111 }, Encoding.ASCII.GetBytes(@"hello WORLD\>") ),

            // Sequences that uncovered errors
            ( new byte[] { 0xb6, 0xb9, 0xac, 0x1c, 0x85, 0x58, 0xd5, 0x20, 0xa4, 0xb6, 0xc2, 0xad, 0x61, 0x7b, 0x5a, 0x54, 0x25, 0x1f }, Encoding.ASCII.GetBytes("upgrade-insecure-requests") ),
            ( new byte[] { 0xfe, 0x53 }, Encoding.ASCII.GetBytes("\"t") ),
            ( new byte[] { 0xb9, 0x49, 0x53, 0x39, 0xe4, 0xb8, 0xa6, 0x2c, 0x1b, 0xff }, Encoding.ASCII.GetBytes(":method: GET") )
        };

        private const int _headerCount = 350; // From line-count in headers.txt
        public static readonly (byte[] encoded, string decodedValue)[] s_headerData = new (byte[], string)[_headerCount];

        [GlobalSetup]
        public void Setup()
        {
            // build the dictionary
            var dict = BuildHuffmanDictionary();
            VerifyHuffmanDictionary(dict);
            HuffmanDict.s_decodingDictionary = dict;

            HuffmanDictOpt.s_decodingDictionary.EnsureCapacity(dict.Count);
            var items = dict.Select(n => new KeyValuePair<uint, HuffmanDictOpt.DecodingTableEntry>(n.Key, new HuffmanDictOpt.DecodingTableEntry(n.Value.DecodedValue, n.Value.BitLength)));
            foreach (var item in items)
                HuffmanDictOpt.s_decodingDictionary.Add(item.Key, item.Value);

            // build the array
            HuffmanArray.VerifyDecodingArray();

            // prepare data to decode
            using (var reader = File.OpenText(@".\HuffmanHeaders.txt"))    // file must be set to copy to output dir for this to work
            {
                var i = 0;
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    var bytes = GetHuffmanEncodedBytes(line);
                    s_headerData[i++] = (bytes, line);
                }
            }
        }

        public HuffmanBench()
        { }

        [Benchmark(Baseline = false, OperationsPerInvoke = (_simpleCount + _headerCount) * _iterations)]
        public ulong Jump()
        {
            var sum = 0ul;

            var rented = ArrayPool<byte>.Shared.Rent(4096);
            {
                for (var j = 0; j < _iterations; j++)
                {
                    // Simple
                    //for (var i = 0; i < s_simpleData.Length; i++)
                    //{
                    //    var encoded = s_simpleData[i].encoded;
                    //    //var expected = _test[i].expected;

                    //    var actualLength = HuffmanJump.Decode(encoded, 0, encoded.Length, rented);
                    //    sum += (uint)actualLength;
                    //}

                    // Headers
                    for (var i = 0; i < s_headerData.Length; i++)
                    {
                        var encoded = s_headerData[i].encoded;

                        var actualLength = HuffmanJump.Decode(encoded, 0, encoded.Length, rented);
                        sum += (uint)actualLength;
                    }
                }
            }
            ArrayPool<byte>.Shared.Return(rented);

            return sum;
        }

        [Benchmark(Baseline = false, OperationsPerInvoke = (_simpleCount + _headerCount) * _iterations)]
        public ulong OrigOpt()
        {
            var sum = 0ul;

            var rented = ArrayPool<byte>.Shared.Rent(4096);
            {
                for (var j = 0; j < _iterations; j++)
                {
                    // Simple
                    for (var i = 0; i < s_simpleData.Length; i++)
                    {
                        var encoded = s_simpleData[i].encoded;
                        //var expected = _test[i].expected;

                        var actualLength = HuffmanOrigOpt.Decode(encoded, 0, encoded.Length, rented);
                        sum += (uint)actualLength;
                    }

                    // Headers
                    for (var i = 0; i < s_headerData.Length; i++)
                    {
                        var encoded = s_headerData[i].encoded;

                        var actualLength = HuffmanOrigOpt.Decode(encoded, 0, encoded.Length, rented);
                        sum += (uint)actualLength;
                    }
                }
            }
            ArrayPool<byte>.Shared.Return(rented);

            return sum;
        }

        //[Benchmark(Baseline = false, OperationsPerInvoke = (_simpleCount + _headerCount) * _iterations)]
        public ulong DictOpt()
        {
            var sum = 0ul;

            var rented = ArrayPool<byte>.Shared.Rent(4096);
            {
                for (var j = 0; j < _iterations; j++)
                {
                    // Simple
                    for (var i = 0; i < s_simpleData.Length; i++)
                    {
                        var encoded = s_simpleData[i].encoded;
                        //var expected = _test[i].expected;

                        var actualLength = HuffmanDictOpt.Decode(encoded, 0, encoded.Length, rented);
                        sum += (uint)actualLength;
                    }

                    // Headers
                    for (var i = 0; i < s_headerData.Length; i++)
                    {
                        var encoded = s_headerData[i].encoded;

                        var actualLength = HuffmanDictOpt.Decode(encoded, 0, encoded.Length, rented);
                        sum += (uint)actualLength;
                    }
                }
            }
            ArrayPool<byte>.Shared.Return(rented);

            return sum;
        }

        //[Benchmark(Baseline = false, OperationsPerInvoke = (_simpleCount + _headerCount) * _iterations)]
        public ulong Dict()
        {
            var sum = 0ul;

            var rented = ArrayPool<byte>.Shared.Rent(4096);
            {
                for (var j = 0; j < _iterations; j++)
                {
                    // Simple
                    //for (var i = 0; i < s_simpleData.Length; i++)
                    //{
                    //    var encoded = s_simpleData[i].encoded;
                    //    //var expected = _test[i].expected;

                    //    var actualLength = HuffmanDict.Decode(encoded, 0, encoded.Length, rented);
                    //    sum += (uint)actualLength;
                    //}

                    // Headers
                    for (var i = 0; i < s_headerData.Length; i++)
                    {
                        var encoded = s_headerData[i].encoded;

                        var actualLength = HuffmanDict.Decode(encoded, 0, encoded.Length, rented);
                        sum += (uint)actualLength;
                    }
                }
            }
            ArrayPool<byte>.Shared.Return(rented);

            return sum;
        }

        [Benchmark(Baseline = true, OperationsPerInvoke = (_simpleCount + _headerCount) * _iterations)]
        public ulong Orig()
        {
            var sum = 0ul;

            var rented = ArrayPool<byte>.Shared.Rent(4096);
            {
                for (var j = 0; j < _iterations; j++)
                {
                    // Simple
                //    for (var i = 0; i < s_simpleData.Length; i++)
                //    {
                //        var encoded = s_simpleData[i].encoded;
                //        //var expected = _test[i].expected;

                //        var actualLength = HuffmanOrig.Decode(encoded, 0, encoded.Length, rented);
                //        sum += (uint)actualLength;
                //    }

                    // Headers
                    for (var i = 0; i < s_headerData.Length; i++)
                    {
                        var encoded = s_headerData[i].encoded;

                        var actualLength = HuffmanOrig.Decode(encoded, 0, encoded.Length, rented);
                        sum += (uint)actualLength;
                    }
                }
            }
            ArrayPool<byte>.Shared.Return(rented);

            return sum;
        }

        [Benchmark(Baseline = false, OperationsPerInvoke = (_simpleCount + _headerCount) * _iterations)]
        public ulong Array()
        {
            var sum = 0ul;

            var rented = ArrayPool<byte>.Shared.Rent(4096);
            {
                for (var j = 0; j < _iterations; j++)
                {
                    // Simple
                    //for (var i = 0; i < s_simpleData.Length; i++)
                    //{
                    //    var encoded = s_simpleData[i].encoded;
                    //    //var expected = _test[i].expected;

                    //    var actualLength = HuffmanArray.Decode(encoded, 0, encoded.Length, rented);
                    //    sum += (uint)actualLength;
                    //}

                    // Headers
                    for (var i = 0; i < s_headerData.Length; i++)
                    {
                        var encoded = s_headerData[i].encoded;
                            
                        var actualLength = HuffmanArray.Decode(encoded, 0, encoded.Length, rented);
                        sum += (uint)actualLength;
                    }
                }
            }
            ArrayPool<byte>.Shared.Return(rented);

            return sum;
        }

        #region Helpers

        private static Dictionary<uint, HuffmanDict.DecodingTableEntry> BuildHuffmanDictionary()
        {
            var dict = new Dictionary<uint, HuffmanDict.DecodingTableEntry>();

            // load all entries from the encoding table. we'll remove items as they are hashed
            var nonHashedEntries = new List<((uint code, int bitLength) tableEntry, uint virtualDictionaryMask, uint decodedValue)>();
            for (uint i = 0; i < HuffmanDict.s_encodingTable.Length; i++)
                nonHashedEntries.Add((HuffmanDict.s_encodingTable[i], 0, i));

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
                            dict.Add(key + j, new HuffmanDict.DecodingTableEntry(entry.decodedValue, tableEntry.bitLength));    // each entry decodes to the same value

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
                            dict.Add(key, new HuffmanDict.DecodingTableEntry(nextVirtualDictionaryMask, 0));
                    }
                }

                workingByte--;

            } while (nonHashedEntries.Count > 0);

            return dict;
        }

        private static void VerifyHuffmanDictionary(Dictionary<uint, HuffmanDict.DecodingTableEntry> dictionary)
        {
            for (uint i = 0; i < HuffmanDict.s_encodingTable.Length; i++)
            {
                (uint code, int bitLength) = HuffmanDict.s_encodingTable[i];
                HuffmanDict.DecodingTableEntry entry;

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

        private static byte[] GetHuffmanEncodedBytes(string value)
        {
            var encodedBytes = new List<byte>();
            byte workingByte = 0;
            int bitsLeftInByte = 8;
            foreach (var character in value)
            {
                var encoded = HuffmanOrig.Encode(character);
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
