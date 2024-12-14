using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Communication_System
{
    public class HuffmanEncoder
    {
        private readonly string text;
        private readonly string outputFilePath;
        private Dictionary<char, string> charTable;

        public HuffmanEncoder(string inputFilePath, string outputFilePath)
        {
            text = File.ReadAllText(inputFilePath);
            this.outputFilePath = outputFilePath;
            charTable = new Dictionary<char, string>();
        }

        private HuffmanNode BuildTree()
        {
            var frequencyMap = new Dictionary<char, int>();

            // Count frequency of each character
            foreach (char c in text)
            {
                if (!frequencyMap.ContainsKey(c))
                    frequencyMap[c] = 0;
                frequencyMap[c]++;
            }

            // Build priority queue
            var priorityQueue = new PriorityQueue<HuffmanNode, int>();
            foreach (var entry in frequencyMap)
            {
                priorityQueue.Enqueue(new HuffmanNode(entry.Value, entry.Key, null, null), entry.Value);
            }

            // Combine nodes
            while (priorityQueue.Count > 1)
            {
                var left = priorityQueue.Dequeue();
                var right = priorityQueue.Dequeue();
                var newNode = new HuffmanNode(left.Freq + right.Freq, '\0', left, right);
                priorityQueue.Enqueue(newNode, newNode.Freq);
            }

            return priorityQueue.Peek();
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
            int paddingSize = (8 - binaryString.Length % 8) % 8;

            // Add padding
            binaryString = binaryString.PadRight(binaryString.Length + paddingSize, '0');

            writer.Write(paddingSize);
            writer.Write(binaryString.Length / 8);

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
