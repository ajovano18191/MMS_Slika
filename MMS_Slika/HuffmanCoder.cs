using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace MMS_Slika
{
    internal static class HuffmanCoder
    {
        private static IHuffmanCoder huffmanCoder = new HuffmanCoder256();

        public static void Code(string filePath, byte[] original, byte wordLength = 8)
        {
            switch(wordLength) {
                case 8:
                    huffmanCoder = new HuffmanCoder256();
                    break;
                case 4:
                    huffmanCoder = new HuffmanCoder16();
                    break;
            }

            Dictionary<byte, BitArray> dictCodes = CreateHuffmanCodes(original);
            byte[] codedBytes = CodeBytes(dictCodes, original);
            Write2File(filePath, wordLength, dictCodes, codedBytes);
        }

        private static Dictionary<byte, int> CreateHuffmanDictionary(byte[] original)
        {
            Dictionary<byte, int> dict = new Dictionary<byte, int>();
            foreach(byte b in original) {
                huffmanCoder.AddOccurenceInDictionary(dict, b);
            }
            return dict;
        }

        private static IHuffmanNode CreateHuffmanTree(Dictionary<byte, int> dict)
        {
            List<IHuffmanNode> huffmanNodes = new List<IHuffmanNode>(dict.Select(p => new HuffmanTerminal {
                Count = p.Value,
                Value = p.Key
            }));

            while(huffmanNodes.Count > 1) {
                huffmanNodes.Sort((a, b) => a.Count - b.Count);
                IHuffmanNode firstEl = huffmanNodes[0];
                IHuffmanNode secondEl = huffmanNodes[1];
                huffmanNodes.RemoveAt(0);
                huffmanNodes.RemoveAt(0);
                huffmanNodes.Add(new HuffmanNode {
                    Count = firstEl.Count + secondEl.Count,
                    Left = firstEl,
                    Right = secondEl,
                });
            }

            return huffmanNodes[0];
        }
        
        private static Dictionary<byte, BitArray> CreateHuffmanCodes(byte[] original)
        {
            Dictionary<byte, int> huffmanDict = CreateHuffmanDictionary(original);
            IHuffmanNode huffmanTreeRoot = CreateHuffmanTree(huffmanDict);

            Dictionary<byte, BitArray> dictCodes = new();
            HuffmanOrder(dictCodes, huffmanTreeRoot, new BitArray(1, true));

            return dictCodes;
        }

        private static void HuffmanOrder(Dictionary<byte, BitArray> dictCodes, IHuffmanNode curr, BitArray code)
        {
            if(curr.GetType() == typeof(HuffmanTerminal)) {
                dictCodes.Add(((HuffmanTerminal)curr).Value, code);
            }
            else {
                HuffmanNode currNode = (HuffmanNode)curr;
                
                BitArray leftCode = (BitArray)code.Clone();
                leftCode.Length++;
                BitArray rightCode = (BitArray)leftCode.Clone();
                rightCode.Set(rightCode.Length - 1, true);

                HuffmanOrder(dictCodes, currNode.Left, leftCode);
                HuffmanOrder(dictCodes, currNode.Right, rightCode);
            }
        }

        private static byte[] CodeBytes(Dictionary<byte, BitArray> dictCodes, byte[] original)
        {
            BitArray codedBitArr = new BitArray(original.Length * 8, false);
            int numOfBits = 0;
            //foreach(byte originalByte in original) {
            for(int i = 0; i < original.Length; i++) {
                byte originalByte = original[i];
                codedBitArr = huffmanCoder.AddCode2BitArray(dictCodes, originalByte, codedBitArr, ref numOfBits);
                // Form1.Instance.Invoke(() => Form1.Instance.Text = $"{i + 1} / {original.Length}");
            }

            codedBitArr.Length = numOfBits;

            byte[] codedBytes = new byte[codedBitArr.Length / 8 + 2];
            codedBytes[codedBytes.Length - 1] = (byte)((codedBytes.Length - 1) * 8 - numOfBits);
            codedBitArr.CopyTo(codedBytes, 0);
            codedBytes = codedBytes.Reverse().ToArray();

            return codedBytes;
        }

        private static void Write2File(string filePath, byte wordLength, Dictionary<byte, BitArray> dictCodes, byte[] codedBytes)
        {
            using var stream = File.Open(filePath, FileMode.Create);
            using var writer = new BinaryWriter(stream);

            writer.Write(wordLength);
            writer.Write(dictCodes.Count);
            foreach(KeyValuePair<byte, BitArray> kv in dictCodes) {
                writer.Write(kv.Key);
                int code = 0;
                for(int i = 0; i < kv.Value.Length; i++) {
                    code <<= 1;
                    code |= Convert.ToInt32(kv.Value[i]);
                }
                writer.Write(code);
            }

            writer.Write(codedBytes);
        }

        public static void AddOccurenceInDictionary(Dictionary<byte, int> dict, byte b)
        {
            if(dict.TryGetValue(b, out int cnt)) {
                dict[b] = cnt + 1;
            }
            else {
                dict.Add(b, 1);
            }
        }

        public static BitArray AddCode2BitArray(Dictionary<byte, BitArray> dictCodes, byte originalByte, BitArray oldCodedBitArr, ref int numOfBits)
        {
            BitArray newCodedBitArr = new BitArray(oldCodedBitArr);
            if(dictCodes.TryGetValue(originalByte, out BitArray? code)) {
                int codeLength = code.Length;
                newCodedBitArr = oldCodedBitArr.LeftShift(codeLength);
                numOfBits += codeLength;
                for(int j = 0; j < codeLength; j++) {
                    newCodedBitArr.Set(j, code[codeLength - 1 - j]);
                }
            }
            return newCodedBitArr;
        }
    }

    internal interface IHuffmanCoder
    {
        public void AddOccurenceInDictionary(Dictionary<byte, int> dict, byte b);
        public BitArray AddCode2BitArray(Dictionary<byte, BitArray> dictCodes, byte originalByte, BitArray oldCodedBitArr, ref int numOfBits);
    }

    internal class HuffmanCoder256 : IHuffmanCoder
    {
        public void AddOccurenceInDictionary(Dictionary<byte, int> dict, byte b)
        {
            HuffmanCoder.AddOccurenceInDictionary(dict, b);
        }

        public BitArray AddCode2BitArray(Dictionary<byte, BitArray> dictCodes, byte originalByte, BitArray oldCodedBitArr, ref int numOfBits)
        {
            return HuffmanCoder.AddCode2BitArray(dictCodes, originalByte, oldCodedBitArr, ref numOfBits);
        }
    }

    internal class HuffmanCoder16 : IHuffmanCoder
    {
        public void AddOccurenceInDictionary(Dictionary<byte, int> dict, byte b)
        {
            HuffmanCoder.AddOccurenceInDictionary(dict, (byte)(b & 0b1111));
            HuffmanCoder.AddOccurenceInDictionary(dict, (byte)(b >> 4));
        }

        public BitArray AddCode2BitArray(Dictionary<byte, BitArray> dictCodes, byte originalByte, BitArray oldCodedBitArr, ref int numOfBits)
        {
            BitArray newCodedBitArr = HuffmanCoder.AddCode2BitArray(dictCodes, (byte)(originalByte >> 4), oldCodedBitArr, ref numOfBits);
            newCodedBitArr = HuffmanCoder.AddCode2BitArray(dictCodes, (byte)(originalByte & 0b1111), newCodedBitArr, ref numOfBits);
            return newCodedBitArr;
        }
    }
}
