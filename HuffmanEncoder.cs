using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HuffmanCoding
{
    public class HuffmanEncoder
    {
        private readonly string text;
        private readonly string outputFilePath;
        private Dictionary<char, string> charTable;

        public HuffmanEncoder(string inputFilePath, string outputFilePath)
        {
            // convert the entire file into one big string
            text = File.ReadAllText(inputFilePath);

            this.outputFilePath = outputFilePath;

            charTable = new Dictionary<char, string>();
        }

        private Dictionary<char, int> CharactersFrequency()
        {
            var frequencyMap = new Dictionary<char, int>();

            // Count frequency of each character
            foreach (char c in text)
            {
                if (!frequencyMap.ContainsKey(c))
                    frequencyMap[c] = 0;

                frequencyMap[c]++;
            }
            return frequencyMap;
        }

        private HuffmanNode BuildTree()
        {
            var frequencyMap = CharactersFrequency();

            // Build priority queue
            var huffmanTree = new PriorityQueue<HuffmanNode, int>();
            foreach (var entry in frequencyMap)
            {
                huffmanTree.Enqueue(new HuffmanNode(entry.Value, entry.Key, null, null), entry.Value);
            }

            // Combine nodes
            while (huffmanTree.Count > 1)
            {
                var left = huffmanTree.Dequeue();
                var right = huffmanTree.Dequeue();
                var parent = new HuffmanNode(left.Freq + right.Freq, '\0', left, right);
                huffmanTree.Enqueue(parent, parent.Freq);
            }

            return huffmanTree.Peek();
        }

        private void BuildCharTable(HuffmanNode root, string code)
        {
            if (root != null)
            {
                if (root.Data != '\0')
                    charTable[root.Data] = code;

                BuildCharTable(root.Left, code + "0");
                BuildCharTable(root.Right, code + "1");
            }
        }

        private void WriteCharTable(BinaryWriter writer)
        {
            writer.Write(charTable.Count);
            foreach (var pair in charTable)
            {
                writer.Write(pair.Key);
                writer.Write(pair.Value.Length);
                writer.Write(pair.Value);
            }
        }

        private void WriteBinaryData(string binaryString, BinaryWriter writer)
        {
            // calc pad size
            int paddingSize = binaryString.Length % 8 == 0 ? 0 : 8 - (binaryString.Length % 8);

            // Add padding
            binaryString = binaryString.PadRight(binaryString.Length + paddingSize, '0');

            // metadata needed at decoding
            writer.Write(paddingSize);
            writer.Write(binaryString.Length / 8);

            // Write the binary data byte by byte
            for (int i = 0; i < binaryString.Length; i += 8)
            {
                byte b = Convert.ToByte(binaryString.Substring(i, 8), 2);
                writer.Write(b);
            }
        }

        private string EncodeText()
        {
            return string.Concat(text.Select(c => charTable[c]));
        }

        public void Encode()
        {
            var tree = BuildTree();
            BuildCharTable(tree, "");

            using var writer = new BinaryWriter(File.Open(outputFilePath, FileMode.Create));
            WriteCharTable(writer);
            WriteBinaryData(EncodeText(), writer);
        }
    }
}
