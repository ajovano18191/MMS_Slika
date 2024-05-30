using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMS_Slika
{
    internal class HuffmanDecoder
    {
        public static byte[] Decode(string filePath)
        {
            LoadFromFile(filePath, out byte wordLength, out Dictionary<int, byte> dictCodes, out byte[] codedBytes);
            byte[] decodedBytes = DecodeBytes(dictCodes, codedBytes);

            if(wordLength == 4) {
                byte[] compressedBytes = new byte[decodedBytes.Length / 2];
                for(int i = 0; i < compressedBytes.Length; i++) {
                    compressedBytes[i] = (byte)((decodedBytes[2 * i] << 4) + decodedBytes[2 * i + 1]);
                }
                decodedBytes = compressedBytes;
            }

            return decodedBytes;
        }

        private static void LoadFromFile(string filePath, out byte wordLength, out Dictionary<int, byte> dictCodes, out byte[] codedBytes)
        {
            using var stream = File.Open(filePath, FileMode.Open);
            using var reader = new BinaryReader(stream);
            wordLength = reader.ReadByte();
            dictCodes = LoadHuffmanCodes(reader);
            codedBytes = LoadCodedBytes(reader, (int)(stream.Length - stream.Position));
        }

        private static Dictionary<int, byte> LoadHuffmanCodes(BinaryReader reader)
        {
            int numOfCodes = reader.ReadInt32();
            Dictionary<int, byte> dictCodes = new Dictionary<int, byte>();

            for(int i = 0; i < numOfCodes; i++) {
                byte value = reader.ReadByte();
                int code = reader.ReadInt32();
                dictCodes.Add(code, value);
            }

            return dictCodes;
        }

        private static byte[] LoadCodedBytes(BinaryReader reader, int numOfCodedBytes)
        {
            return reader.ReadBytes(numOfCodedBytes);
        }

        private static byte[] DecodeBytes(Dictionary<int, byte> dictCodes, byte[] codedBytes)
        {
            List<byte> decodedBytes = [];

            int bitInd = codedBytes[0];
            int byteIndex = 1;
            byte currByte = ReverseBits(codedBytes, ref byteIndex);
            currByte >>= bitInd;
            int currCode = 0;

            while(true) {
                currCode = NextCode(currCode, ref currByte, ref bitInd);

                if(dictCodes.TryGetValue(currCode, out byte value)) {
                    decodedBytes.Add(value);
                    currCode = 0;
                }

                if(bitInd >= 8) {
                    if(byteIndex >= codedBytes.Length) {
                        break;
                    }
                    currByte = ReverseBits(codedBytes, ref byteIndex);
                    bitInd = 0;
                }
            }
            return [..decodedBytes];
        }

        private static byte ReverseBits(byte[] bytes, ref int byteIndex)
        {
            byte orgByte = bytes[byteIndex];
            byteIndex++;

            byte revByte = 0;
            for(int j = 0; j < 8; j++) {
                revByte <<= 1;
                revByte |= (byte)(orgByte & 1);
                orgByte >>= 1;
            }

            return revByte;
        }

        private static int NextCode(int currCode, ref byte currByte, ref int bitInd)
        {
            currCode <<= 1;
            currCode |= currByte & 1;
            currByte >>= 1;
            bitInd++;
            return currCode;
        }
    }
}
