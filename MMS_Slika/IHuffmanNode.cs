using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMS_Slika
{
    internal interface IHuffmanNode
    {
        public int Count { get; set; }
        // public IHuffmanNode Value { get; set; }
    }

    internal class HuffmanTerminal : IHuffmanNode
    {
        public int Count { get; set; } = 0;
        public byte Value { get; set; } = 0;
    }

    internal class HuffmanNode : IHuffmanNode
    {
        public int Count { get; set; } = 0;
        public IHuffmanNode Left { get; set; } = new HuffmanTerminal();

        public IHuffmanNode Right { get; set; } = new HuffmanTerminal();
    }
}
