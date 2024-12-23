using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Communication_System.HuffmanCoding
{
    public class HuffmanNode
    {
        public int Freq { get; }
        public char Data { get; }
        public HuffmanNode Left { get; }
        public HuffmanNode Right { get; }

        public HuffmanNode(int freq, char data, HuffmanNode left = null, HuffmanNode right = null)
        {
            Freq = freq;
            Data = data;
            Left = left;
            Right = right;
        }


    }
}
