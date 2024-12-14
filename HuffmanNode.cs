using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Communication_System
{
    internal class HuffmanNode
    {
        public int Freq { get; set; }  // Frequency of the character
        public char Data { get; set; } = '\0';  // The character (default is null character)
        public HuffmanNode Left { get; set; } = null;  // Left child node
        public HuffmanNode Right { get; set; } = null;  // Right child node

        public HuffmanNode(int freq, char data, HuffmanNode left, HuffmanNode right)
        {
            Freq = freq;
            Data = data;
            Left = left;
            Right = right;
        }

    }
}
