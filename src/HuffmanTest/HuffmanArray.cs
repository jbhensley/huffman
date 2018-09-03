﻿using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace HuffmanTest
{
    internal class HuffmanArray
    {
        private static readonly (uint code, int bitLength)[] s_encodingTable = new (uint code, int bitLength)[]
        {
            (0b11111111_11000000_00000000_00000000, 13),
            (0b11111111_11111111_10110000_00000000, 23),
            (0b11111111_11111111_11111110_00100000, 28),
            (0b11111111_11111111_11111110_00110000, 28),
            (0b11111111_11111111_11111110_01000000, 28),
            (0b11111111_11111111_11111110_01010000, 28),
            (0b11111111_11111111_11111110_01100000, 28),
            (0b11111111_11111111_11111110_01110000, 28),
            (0b11111111_11111111_11111110_10000000, 28),
            (0b11111111_11111111_11101010_00000000, 24),
            (0b11111111_11111111_11111111_11110000, 30),
            (0b11111111_11111111_11111110_10010000, 28),
            (0b11111111_11111111_11111110_10100000, 28),
            (0b11111111_11111111_11111111_11110100, 30),
            (0b11111111_11111111_11111110_10110000, 28),
            (0b11111111_11111111_11111110_11000000, 28),
            (0b11111111_11111111_11111110_11010000, 28),
            (0b11111111_11111111_11111110_11100000, 28),
            (0b11111111_11111111_11111110_11110000, 28),
            (0b11111111_11111111_11111111_00000000, 28),
            (0b11111111_11111111_11111111_00010000, 28),
            (0b11111111_11111111_11111111_00100000, 28),
            (0b11111111_11111111_11111111_11111000, 30),
            (0b11111111_11111111_11111111_00110000, 28),
            (0b11111111_11111111_11111111_01000000, 28),
            (0b11111111_11111111_11111111_01010000, 28),
            (0b11111111_11111111_11111111_01100000, 28),
            (0b11111111_11111111_11111111_01110000, 28),
            (0b11111111_11111111_11111111_10000000, 28),
            (0b11111111_11111111_11111111_10010000, 28),
            (0b11111111_11111111_11111111_10100000, 28),
            (0b11111111_11111111_11111111_10110000, 28),
            (0b01010000_00000000_00000000_00000000, 6),
            (0b11111110_00000000_00000000_00000000, 10),
            (0b11111110_01000000_00000000_00000000, 10),
            (0b11111111_10100000_00000000_00000000, 12),
            (0b11111111_11001000_00000000_00000000, 13),
            (0b01010100_00000000_00000000_00000000, 6),
            (0b11111000_00000000_00000000_00000000, 8),
            (0b11111111_01000000_00000000_00000000, 11),
            (0b11111110_10000000_00000000_00000000, 10),
            (0b11111110_11000000_00000000_00000000, 10),
            (0b11111001_00000000_00000000_00000000, 8),
            (0b11111111_01100000_00000000_00000000, 11),
            (0b11111010_00000000_00000000_00000000, 8),
            (0b01011000_00000000_00000000_00000000, 6),
            (0b01011100_00000000_00000000_00000000, 6),
            (0b01100000_00000000_00000000_00000000, 6),
            (0b00000000_00000000_00000000_00000000, 5),
            (0b00001000_00000000_00000000_00000000, 5),
            (0b00010000_00000000_00000000_00000000, 5),
            (0b01100100_00000000_00000000_00000000, 6),
            (0b01101000_00000000_00000000_00000000, 6),
            (0b01101100_00000000_00000000_00000000, 6),
            (0b01110000_00000000_00000000_00000000, 6),
            (0b01110100_00000000_00000000_00000000, 6),
            (0b01111000_00000000_00000000_00000000, 6),
            (0b01111100_00000000_00000000_00000000, 6),
            (0b10111000_00000000_00000000_00000000, 7),
            (0b11111011_00000000_00000000_00000000, 8),
            (0b11111111_11111000_00000000_00000000, 15),
            (0b10000000_00000000_00000000_00000000, 6),
            (0b11111111_10110000_00000000_00000000, 12),
            (0b11111111_00000000_00000000_00000000, 10),
            (0b11111111_11010000_00000000_00000000, 13),
            (0b10000100_00000000_00000000_00000000, 6),
            (0b10111010_00000000_00000000_00000000, 7),
            (0b10111100_00000000_00000000_00000000, 7),
            (0b10111110_00000000_00000000_00000000, 7),
            (0b11000000_00000000_00000000_00000000, 7),
            (0b11000010_00000000_00000000_00000000, 7),
            (0b11000100_00000000_00000000_00000000, 7),
            (0b11000110_00000000_00000000_00000000, 7),
            (0b11001000_00000000_00000000_00000000, 7),
            (0b11001010_00000000_00000000_00000000, 7),
            (0b11001100_00000000_00000000_00000000, 7),
            (0b11001110_00000000_00000000_00000000, 7),
            (0b11010000_00000000_00000000_00000000, 7),
            (0b11010010_00000000_00000000_00000000, 7),
            (0b11010100_00000000_00000000_00000000, 7),
            (0b11010110_00000000_00000000_00000000, 7),
            (0b11011000_00000000_00000000_00000000, 7),
            (0b11011010_00000000_00000000_00000000, 7),
            (0b11011100_00000000_00000000_00000000, 7),
            (0b11011110_00000000_00000000_00000000, 7),
            (0b11100000_00000000_00000000_00000000, 7),
            (0b11100010_00000000_00000000_00000000, 7),
            (0b11100100_00000000_00000000_00000000, 7),
            (0b11111100_00000000_00000000_00000000, 8),
            (0b11100110_00000000_00000000_00000000, 7),
            (0b11111101_00000000_00000000_00000000, 8),
            (0b11111111_11011000_00000000_00000000, 13),
            (0b11111111_11111110_00000000_00000000, 19),
            (0b11111111_11100000_00000000_00000000, 13),
            (0b11111111_11110000_00000000_00000000, 14),
            (0b10001000_00000000_00000000_00000000, 6),
            (0b11111111_11111010_00000000_00000000, 15),
            (0b00011000_00000000_00000000_00000000, 5),
            (0b10001100_00000000_00000000_00000000, 6),
            (0b00100000_00000000_00000000_00000000, 5),
            (0b10010000_00000000_00000000_00000000, 6),
            (0b00101000_00000000_00000000_00000000, 5),
            (0b10010100_00000000_00000000_00000000, 6),
            (0b10011000_00000000_00000000_00000000, 6),
            (0b10011100_00000000_00000000_00000000, 6),
            (0b00110000_00000000_00000000_00000000, 5),
            (0b11101000_00000000_00000000_00000000, 7),
            (0b11101010_00000000_00000000_00000000, 7),
            (0b10100000_00000000_00000000_00000000, 6),
            (0b10100100_00000000_00000000_00000000, 6),
            (0b10101000_00000000_00000000_00000000, 6),
            (0b00111000_00000000_00000000_00000000, 5),
            (0b10101100_00000000_00000000_00000000, 6),
            (0b11101100_00000000_00000000_00000000, 7),
            (0b10110000_00000000_00000000_00000000, 6),
            (0b01000000_00000000_00000000_00000000, 5),
            (0b01001000_00000000_00000000_00000000, 5),
            (0b10110100_00000000_00000000_00000000, 6),
            (0b11101110_00000000_00000000_00000000, 7),
            (0b11110000_00000000_00000000_00000000, 7),
            (0b11110010_00000000_00000000_00000000, 7),
            (0b11110100_00000000_00000000_00000000, 7),
            (0b11110110_00000000_00000000_00000000, 7),
            (0b11111111_11111100_00000000_00000000, 15),
            (0b11111111_10000000_00000000_00000000, 11),
            (0b11111111_11110100_00000000_00000000, 14),
            (0b11111111_11101000_00000000_00000000, 13),
            (0b11111111_11111111_11111111_11000000, 28),
            (0b11111111_11111110_01100000_00000000, 20),
            (0b11111111_11111111_01001000_00000000, 22),
            (0b11111111_11111110_01110000_00000000, 20),
            (0b11111111_11111110_10000000_00000000, 20),
            (0b11111111_11111111_01001100_00000000, 22),
            (0b11111111_11111111_01010000_00000000, 22),
            (0b11111111_11111111_01010100_00000000, 22),
            (0b11111111_11111111_10110010_00000000, 23),
            (0b11111111_11111111_01011000_00000000, 22),
            (0b11111111_11111111_10110100_00000000, 23),
            (0b11111111_11111111_10110110_00000000, 23),
            (0b11111111_11111111_10111000_00000000, 23),
            (0b11111111_11111111_10111010_00000000, 23),
            (0b11111111_11111111_10111100_00000000, 23),
            (0b11111111_11111111_11101011_00000000, 24),
            (0b11111111_11111111_10111110_00000000, 23),
            (0b11111111_11111111_11101100_00000000, 24),
            (0b11111111_11111111_11101101_00000000, 24),
            (0b11111111_11111111_01011100_00000000, 22),
            (0b11111111_11111111_11000000_00000000, 23),
            (0b11111111_11111111_11101110_00000000, 24),
            (0b11111111_11111111_11000010_00000000, 23),
            (0b11111111_11111111_11000100_00000000, 23),
            (0b11111111_11111111_11000110_00000000, 23),
            (0b11111111_11111111_11001000_00000000, 23),
            (0b11111111_11111110_11100000_00000000, 21),
            (0b11111111_11111111_01100000_00000000, 22),
            (0b11111111_11111111_11001010_00000000, 23),
            (0b11111111_11111111_01100100_00000000, 22),
            (0b11111111_11111111_11001100_00000000, 23),
            (0b11111111_11111111_11001110_00000000, 23),
            (0b11111111_11111111_11101111_00000000, 24),
            (0b11111111_11111111_01101000_00000000, 22),
            (0b11111111_11111110_11101000_00000000, 21),
            (0b11111111_11111110_10010000_00000000, 20),
            (0b11111111_11111111_01101100_00000000, 22),
            (0b11111111_11111111_01110000_00000000, 22),
            (0b11111111_11111111_11010000_00000000, 23),
            (0b11111111_11111111_11010010_00000000, 23),
            (0b11111111_11111110_11110000_00000000, 21),
            (0b11111111_11111111_11010100_00000000, 23),
            (0b11111111_11111111_01110100_00000000, 22),
            (0b11111111_11111111_01111000_00000000, 22),
            (0b11111111_11111111_11110000_00000000, 24),
            (0b11111111_11111110_11111000_00000000, 21),
            (0b11111111_11111111_01111100_00000000, 22),
            (0b11111111_11111111_11010110_00000000, 23),
            (0b11111111_11111111_11011000_00000000, 23),
            (0b11111111_11111111_00000000_00000000, 21),
            (0b11111111_11111111_00001000_00000000, 21),
            (0b11111111_11111111_10000000_00000000, 22),
            (0b11111111_11111111_00010000_00000000, 21),
            (0b11111111_11111111_11011010_00000000, 23),
            (0b11111111_11111111_10000100_00000000, 22),
            (0b11111111_11111111_11011100_00000000, 23),
            (0b11111111_11111111_11011110_00000000, 23),
            (0b11111111_11111110_10100000_00000000, 20),
            (0b11111111_11111111_10001000_00000000, 22),
            (0b11111111_11111111_10001100_00000000, 22),
            (0b11111111_11111111_10010000_00000000, 22),
            (0b11111111_11111111_11100000_00000000, 23),
            (0b11111111_11111111_10010100_00000000, 22),
            (0b11111111_11111111_10011000_00000000, 22),
            (0b11111111_11111111_11100010_00000000, 23),
            (0b11111111_11111111_11111000_00000000, 26),
            (0b11111111_11111111_11111000_01000000, 26),
            (0b11111111_11111110_10110000_00000000, 20),
            (0b11111111_11111110_00100000_00000000, 19),
            (0b11111111_11111111_10011100_00000000, 22),
            (0b11111111_11111111_11100100_00000000, 23),
            (0b11111111_11111111_10100000_00000000, 22),
            (0b11111111_11111111_11110110_00000000, 25),
            (0b11111111_11111111_11111000_10000000, 26),
            (0b11111111_11111111_11111000_11000000, 26),
            (0b11111111_11111111_11111001_00000000, 26),
            (0b11111111_11111111_11111011_11000000, 27),
            (0b11111111_11111111_11111011_11100000, 27),
            (0b11111111_11111111_11111001_01000000, 26),
            (0b11111111_11111111_11110001_00000000, 24),
            (0b11111111_11111111_11110110_10000000, 25),
            (0b11111111_11111110_01000000_00000000, 19),
            (0b11111111_11111111_00011000_00000000, 21),
            (0b11111111_11111111_11111001_10000000, 26),
            (0b11111111_11111111_11111100_00000000, 27),
            (0b11111111_11111111_11111100_00100000, 27),
            (0b11111111_11111111_11111001_11000000, 26),
            (0b11111111_11111111_11111100_01000000, 27),
            (0b11111111_11111111_11110010_00000000, 24),
            (0b11111111_11111111_00100000_00000000, 21),
            (0b11111111_11111111_00101000_00000000, 21),
            (0b11111111_11111111_11111010_00000000, 26),
            (0b11111111_11111111_11111010_01000000, 26),
            (0b11111111_11111111_11111111_11010000, 28),
            (0b11111111_11111111_11111100_01100000, 27),
            (0b11111111_11111111_11111100_10000000, 27),
            (0b11111111_11111111_11111100_10100000, 27),
            (0b11111111_11111110_11000000_00000000, 20),
            (0b11111111_11111111_11110011_00000000, 24),
            (0b11111111_11111110_11010000_00000000, 20),
            (0b11111111_11111111_00110000_00000000, 21),
            (0b11111111_11111111_10100100_00000000, 22),
            (0b11111111_11111111_00111000_00000000, 21),
            (0b11111111_11111111_01000000_00000000, 21),
            (0b11111111_11111111_11100110_00000000, 23),
            (0b11111111_11111111_10101000_00000000, 22),
            (0b11111111_11111111_10101100_00000000, 22),
            (0b11111111_11111111_11110111_00000000, 25),
            (0b11111111_11111111_11110111_10000000, 25),
            (0b11111111_11111111_11110100_00000000, 24),
            (0b11111111_11111111_11110101_00000000, 24),
            (0b11111111_11111111_11111010_10000000, 26),
            (0b11111111_11111111_11101000_00000000, 23),
            (0b11111111_11111111_11111010_11000000, 26),
            (0b11111111_11111111_11111100_11000000, 27),
            (0b11111111_11111111_11111011_00000000, 26),
            (0b11111111_11111111_11111011_01000000, 26),
            (0b11111111_11111111_11111100_11100000, 27),
            (0b11111111_11111111_11111101_00000000, 27),
            (0b11111111_11111111_11111101_00100000, 27),
            (0b11111111_11111111_11111101_01000000, 27),
            (0b11111111_11111111_11111101_01100000, 27),
            (0b11111111_11111111_11111111_11100000, 28),
            (0b11111111_11111111_11111101_10000000, 27),
            (0b11111111_11111111_11111101_10100000, 27),
            (0b11111111_11111111_11111101_11000000, 27),
            (0b11111111_11111111_11111101_11100000, 27),
            (0b11111111_11111111_11111110_00000000, 27),
            (0b11111111_11111111_11111011_10000000, 26),
            (0b11111111_11111111_11111111_11111100, 30)
        };

        private static readonly Lazy<int[,]> s_decodingArrayLoader = new Lazy<int[,]>(() => BuildDecodingArray());
        private static int[,] s_decodingArray => s_decodingArrayLoader.Value;

        public static (uint encoded, int bitLength) Encode(int data)
        {
            return s_encodingTable[data];
        }

        /// <summary>
        /// Decodes a Huffman encoded string from a byte array.
        /// </summary>
        /// <param name="src">The source byte array containing the encoded data.</param>
        /// <param name="offsets">The offset in the byte array where the coded data starts.</param>
        /// <param name="count">The number of bytes to decode.</param>
        /// <param name="dst">The destination byte array to store the decoded data.</param>
        // <returns>The number of decoded symbols.</returns>
        public static int Decode(byte[] src, int offset, int count, byte[] dst)
        {
            // TODO: should we put argument checks here? (e.g. -1 < offset < src.Length; count > -1; src != null; dst != null; etc)
            //       i assume values like count==0 are valid, even though they might not make sense in terms of calling
            //  

            // let's narrow thing down to just the part of the source buffer that we've been asked to decode
            var sourceSpan = new Span<byte>(src, offset, count);
            int sourceIndex = 0;
            int destinationIndex = 0;
            int decodedBytes = 0;
            int bitOffset = 0;
            while (sourceIndex < sourceSpan.Length)
            {
                // decode in a max of 4
                int arrayIndex = 0;
                bool didDecode = false;

                for (int i = 0; i < 4; i++)
                {
                    // grab the next byte and push it over to make room for bits we have to borrow
                    byte workingByte = (byte)(sourceSpan[sourceIndex] << bitOffset);
                    // borrow some bits if we need to
                    if (bitOffset > 0)
                        workingByte |= (byte)((sourceIndex < sourceSpan.Length - 1 ? sourceSpan[sourceIndex + 1] : 0) >> (8 - bitOffset));



                    // attempt to decode
                    int decodedValue = Decode(workingByte, arrayIndex, out int decodedBits);

                    // negative values are a pointer to the next decoding array
                    if (decodedValue < 0)
                    {
                        // move to the next source byte and prepare to key in again
                        sourceIndex++;
                        arrayIndex = -decodedValue;
                    }
                    else
                    // we have decoded a character
                    {
                        // A Huffman-encoded string literal containing the EOS symbol MUST be treated as a decoding error.
                        // http://httpwg.org/specs/rfc7541.html#rfc.section.5.2
                        if (decodedValue > 256)
                            throw new HuffmanDecodingException();

                        // Destination is too small.
                        if (destinationIndex == dst.Length)
                            throw new HuffmanDecodingException();

                        // store the decoded value and increment total decoded byte count
                        dst[destinationIndex++] = (byte)decodedValue;
                        decodedBytes++;

                        // if we decoded more bits than we are borrowing then advance to next byte
                        if (decodedBits - (8 * i) >= 8 - bitOffset)
                            sourceIndex++;

                        // calculate and re-align bitOffset
                        bitOffset += decodedBits;
                        bitOffset &= 7; // same as modulo 8

                        didDecode = true;

                        break;
                    }
                }

                // we checked four bytes did not come up with a valid bit pattern
                if (!didDecode)
                    throw new HuffmanDecodingException();

                // check for padding in the last byte before we loop back around and try to decode again
                /*
                 *      TODO: review this logic
                 *            padding is used to re-align with byte boundary at the end. if we take previous
                 *            byte alignment out of the equation then this is simple: a final byte consisting
                 *            of 8 set bits is an error; it should have just been left out entirely. things get
                 *            messy when we start dealing with offset. consider:
                 *            
                 *            src[len-2] = 0101 1111     -- the first four being data and the last four padding
                 *            
                 *            but then the caller adds one more byte:
                 *            
                 *            src[len-1] = 1111 0000     -- first four are more padding and the last four are zeros for whatever reason
                 *            
                 *            technically, there is a padding sequence of 8 bits here. however, that last byte
                 *            should not have even been added since it was not necessary and carries no data.
                 *            
                 *            i think that what we are really checking for is whether the remaning bits are
                 *            padding so we know to terminate the loop and to ensure that the caller did not
                 *            fully pad an entire byte because that is unnecessary. a sequence like that above
                 *            will either toss a decoding error or, possibly, decode to an unintended character.
                 *            the question is: should we look for that specific sceanrio and force a decoding error?
                 */
                if (sourceIndex == sourceSpan.Length - 1)
                {
                    
                    // see if all of the remaning bits are padding
                    int paddingMask = (0x1 << (8 - bitOffset)) - 1;
                    if ((sourceSpan[sourceIndex] & paddingMask) == paddingMask)
                    {
                        // "A padding strictly longer than 7 bits MUST be treated as a decoding error."
                        // https://httpwg.org/specs/rfc7541.html#rfc.section.5.2

                        //  **** however, see comment above. is it still considered padding if zeros follow? ****
                        if (bitOffset == 0)
                            throw new HuffmanDecodingException();

                        break;
                    }
                }
            }

            return decodedBytes;
            
        }
        public static int Decode(byte data, int arrayIndex, out int decodedBits)
        {
            decodedBits = 0;
            
            // key into array
            int value = s_decodingArray[arrayIndex, data];

            // if the value is positive then we have the bit length and character code
            if (value > -1)
            {
                int bitLength = value >> 8; // length is in the second LSB

                decodedBits = bitLength;
                return value & 0xFF;   // value is in the LSB
            }

            // pointer to the next array will be stored as a negative
            return value;
        }
        public static int DecodeNew(byte[] src, int offset, int count, byte[] dst)
        {
            // TODO: should we put argument checks here? (e.g. offset > -1; count > -1; src != null; dst != null; etc)
            //       i assume values like count==0 are valid, even though they might not make sense in terms of calling
            //       the method when you don't actually want anything back

            int dstIndex = 0;
            int bitOffset = 0;
            int totalDecodedBytes = 0;

            /*
             *   TODO:    resolve how we should handle count here      
             *      we should honor the caller's request to only decode a certain number of bytes. however, how does
             *      borrowing bits from the neighboring byte fit in to that? let's say that we're working on the last byte
             *      that count allows us to take, but we have already borrowed bits from it to fill the byte immediately
             *      before it. essentially, we no longer have a full byte. do we borrow from the next byte in the buffer
             *      or do we consider it off limits? if it is off limits, do we toss a decoding error because the last byte
             *      could not be decoded by itself? it's no longer clear what "decode only 4 bytes" means when we are crossing
             *      byte boundaries
             */

            // loop until we either reach the end of the buffer, we've decoded the specified count of bytes or we hit the end of the destination buffer
            while (offset < src.Length &&  totalDecodedBytes < count && dstIndex < dst.Length)    // it's possible for the caller to specify count=0, in which case we should decoding nothing
            {   
                int arrayIndex = 0;     // index into decoding table array
                int length = 0;

                // decode in a max of 4
                for (int i = 0; i < 4; i++)
                {
                    // grab the working byte and make room for any bits we have to borrow from its neighbor
                    byte byteValue = (byte)(src[offset] << bitOffset);

                    // borrow some bits form the neighbor (or pad with ones if we're on the last byte) if we're not aligned to a byte
                    if (bitOffset > 0)
                        byteValue |= (byte)(((offset < src.Length - 1) ? src[offset + 1] : int.MinValue) >> (8 - bitOffset)); // e.g. 0b10101101 and borrow 5 means shift him to the right by 3 to get 0b00010101 and | into the slot already provided above

                    // key into array
                    int value = s_decodingArray[arrayIndex, byteValue];

                    // if the value is positive then we have the bit length and character code
                    if (value > -1)
                    {
                        length = value >> 8; // length is in the second LSB
                        bitOffset += length - (i * 8);

                        // store the value and increment the destination index after
                        dst[dstIndex++] = (byte)(value & 0xFF);    // value is in the LSB

                        // if our offset crosses a byte boundary then we move to the next byte and reseet
                        if (bitOffset >= 8)
                        {
                            offset++;
                            bitOffset &= 7;
                            totalDecodedBytes++;
                        }

                        break;
                    }

                    // pointer to the next array will be stored as a negative
                    arrayIndex = -value;
                    totalDecodedBytes++;

                    // if we got a pointer to the next array then we consumed all of the remaining bits in the working byte.
                    // move on to the next byte, but make sure we don't go past the end of the buffer.
                    // we will still need to continue to borrow the same number bits from our neighbor if we were borrowing before
                    if (++offset >= src.Length)
                        break;  // TODO: need to fix case when 8 or more bits of padding are present. this will require that we keep track of
                                // how many consecutive bits are set as we tunnel our way through the array
                }

                // No valid symbol could be decoded with the bits. This also includes cases where the symbol is EOS (which we don't store in the decoding table)
                // This squares up with the original implementation, which contained this comment:            
                //       A Huffman-encoded string literal containing the EOS symbol MUST be treated as a decoding error.
                //       http://httpwg.org/specs/rfc7541.html#rfc.section.5.2
                if (length == 0)
                    throw new HuffmanDecodingException();
            }

            // see if we hit the end of the source buffer or the destination buffer before we could decode   // all of the values
            if(totalDecodedBytes + (bitOffset > 0 ? 1 : 0) < count)
                throw new HuffmanDecodingException();

            return dstIndex;
        }

        /// <summary>
        /// Decodes a single symbol from a 32-bit word.
        /// </summary>
        /// <param name="data">A 32-bit word containing a Huffman encoded symbol.</param>
        /// <param name="validBits">
        /// The number of bits in <paramref name="data"/> that may contain an encoded symbol.
        /// This is not the exact number of bits that encode the symbol. Instead, it prevents
        /// decoding the lower bits of <paramref name="data"/> if they don't contain any
        /// encoded data.
        /// </param>
        /// <param name="decodedBits">The number of bits decoded from <paramref name="data"/>.</param>
        /// <returns>The decoded symbol.</returns>
        public static int Decode(uint data, int validBits, out int decodedBits)
        {
            // decode in a max of 4
            int arrayIndex = 0;
            for (int i = 0; i < 4; i++)
            {
                // grab data one byte at a time starting at the left
                uint workingByte = data >> (8 * (3 - i)) & 0xFF;

                // key into array
                int value = s_decodingArray[arrayIndex, workingByte];

                // if the value is positive then we have the bit length and character code
                if (value > -1)
                {
                    int bitLength = value >> 8; // length is in the second LSB
                    if (bitLength > validBits)
                        break;  // we only found a value by incorporating bits beyond the the valid remaining length of the data stream

                    decodedBits = bitLength;
                    return value & 0xFF;   // value is in the LSB
                }

                // pointer to the next array will be stored as a negative
                arrayIndex = -value;
            }

            // no luck. signal to caller that we could not decode
            decodedBits = 0;
            return -1;
        }

        private static int[,] BuildDecodingArray()
        {
            var array = new int[15, 256];
            
            int nextAvailableSubIndex = 1;
            // loop through each entry in the encoding table and create entries for it in our decoding array
            for (int i = 0; i < 256; i++)   // we're only going to 255. the EOS pattern of 256 (which will not fit into a byte) will be left out.
            {
                var tableEntry = s_encodingTable[i];    // keep from having to do "s_encodingTable[i]" everywhere
                int currentArrayIndex = 0;              // which array are we working with

                // loop for however many bytes the value occupies
                for (int j = 0; j <= Math.Ceiling(tableEntry.bitLength / 8.0); j++)
                {
                    int byteOffset = 8 * (3 - j);       // how many bits is the working byte offset from the right
                    int totalLength = 8 * (j + 1);      // how many bits of the entry can consume total so far

                    uint codeByte = (tableEntry.code >> byteOffset) & 0xFF;  // extract the working byte and shift it all the way to the right

                    // we can finish the entry this time around. store the remaning bits and bail on the loop
                    if (tableEntry.bitLength <= totalLength)
                    {
                        // we need to store all permutations of the bits that are beyond the length of the code
                        int loopMax = 0x1 << (totalLength - tableEntry.bitLength); // have to create entries for all of these values
                        int value = (tableEntry.bitLength << 8) | i;    // store the length and value in two separate positions
                        for (uint k = 0; k < loopMax; k++)
                            array[currentArrayIndex, codeByte + k] = value;   // each entry returns the same value

                        break;  // we're done with this entry. bail on the loop
                    }
                    // else: we need to split the entry into one or more sub-arrays

                    // let's see if anyone before us has already claimed a sub-array with our bit pattern
                    int subArrayIndex = array[currentArrayIndex, codeByte];

                    // negative values are used as pointers to the next array. zeros are unused. positive values are a successful decode
                    if (subArrayIndex < 0)
                        subArrayIndex = -subArrayIndex;
                    else
                    {
                        subArrayIndex = nextAvailableSubIndex++;    // if no one has our bit battern then we'll stake our claim on the next available array
                        array[currentArrayIndex, codeByte] = -subArrayIndex;  // blaze the trail for the next guy
                    }

                    currentArrayIndex = subArrayIndex;  // we've left a pointer behind us and we're moving on to the next array
                }
            }

            return array;
        }

        public static void VerifyDecodingArray()
        {
            for (int i = 0; i < s_encodingTable.Length - 1; i++)
            {
                (uint code, int bitLength) = s_encodingTable[i];
                int decoded = Decode(code, bitLength, out int decodedBits);
                Assert.NotEqual(-1, decoded);
                Assert.Equal(bitLength, decodedBits);
                Assert.Equal(i, decoded);
            }
        }
    }
}