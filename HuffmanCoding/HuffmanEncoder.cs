using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Communication_System.HuffmanCoding
{
    public class HuffmanEncoder : IDisposable
    {
        private readonly string inputFilePath;
        private readonly string outputFilePath;
        private Dictionary<char, string> charTable;
        private const int MAX_FILE_SIZE = 100 * 1024 * 1024; // 100MB limit

        public HuffmanEncoder(string inputFilePath, string outputFilePath)
        {
            this.inputFilePath = inputFilePath ?? throw new ArgumentNullException(nameof(inputFilePath));
            this.outputFilePath = outputFilePath ?? throw new ArgumentNullException(nameof(outputFilePath));
            charTable = new Dictionary<char, string>();
        }

        public void Encode()
        {
            ValidateInputFile();
            var tree = BuildTree();
            BuildCharTable(tree, "");

            using var writer = new BinaryWriter(File.Open(outputFilePath, FileMode.Create));
            WriteCharTable(writer);
            EncodeAndWriteText(writer);
        }

        private void ValidateInputFile()
        {
            var fileInfo = new FileInfo(inputFilePath);
            if (!fileInfo.Exists)
                throw new FileNotFoundException("Input file not found", inputFilePath);
            if (fileInfo.Length > MAX_FILE_SIZE)
                throw new InvalidOperationException($"File size exceeds maximum limit of {MAX_FILE_SIZE / (1024 * 1024)}MB");
        }


        public Dictionary<char, int> CharactersFrequency()
        {
            var frequencyMap = new Dictionary<char, int>();
            using var reader = new StreamReader(inputFilePath);

            int character;
            while ((character = reader.Read()) != -1)
            {
                char c = (char)character;
                if (!frequencyMap.ContainsKey(c))
                    frequencyMap[c] = 0;
                frequencyMap[c]++;
            }

            // Calculate total characters and display probabilities
            int totalCharacters = frequencyMap.Values.Sum();
            Console.WriteLine("Symbol Probabilities:");
            foreach (var pair in frequencyMap)
            {
                double probability = (double)pair.Value / totalCharacters;
                Console.WriteLine($"'{pair.Key}': {probability:F4}");
            }


            return frequencyMap;
        }

        private HuffmanNode BuildTree()
        {
            var frequencyMap = CharactersFrequency();
            if (frequencyMap.Count == 0)
                throw new InvalidOperationException("Input file is empty");

            var huffmanTree = new PriorityQueue<HuffmanNode, int>();
            foreach (var entry in frequencyMap)
            {
                huffmanTree.Enqueue(new HuffmanNode(entry.Value, entry.Key), entry.Value);
            }

            while (huffmanTree.Count > 1)
            {
                var left = huffmanTree.Dequeue();
                var right = huffmanTree.Dequeue();
                var parent = new HuffmanNode(left.Freq + right.Freq, '\0', left, right);
                huffmanTree.Enqueue(parent, parent.Freq);
            }

            return huffmanTree.Dequeue();
        }

        private void BuildCharTable(HuffmanNode root, string code, int depth = 0)
        {
            const int MAX_TREE_DEPTH = 1000;

            if (depth > MAX_TREE_DEPTH)
                throw new InvalidOperationException("Huffman tree exceeded maximum depth");

            if (root != null)
            {
                if (root.Data != '\0')
                    charTable[root.Data] = code;
                BuildCharTable(root.Left, code + "0", depth + 1);
                BuildCharTable(root.Right, code + "1", depth + 1);
            }
        }

        private void WriteCharTable(BinaryWriter writer)
        {
            writer.Write(charTable.Count);
            foreach (var pair in charTable)
            {
                writer.Write(pair.Key);
                writer.Write(pair.Value);
            }
        }

        private void EncodeAndWriteText(BinaryWriter writer)
        {
            using var reader = new StreamReader(inputFilePath);
            var buffer = new StringBuilder();
            const int BUFFER_SIZE = 8192;

            int character;
            while ((character = reader.Read()) != -1)
            {
                buffer.Append(charTable[(char)character]);

                if (buffer.Length >= BUFFER_SIZE)
                {
                    WriteBinaryBuffer(buffer.ToString(), writer, false);
                    buffer.Clear();
                }
            }

            if (buffer.Length > 0)
            {
                WriteBinaryBuffer(buffer.ToString(), writer, true);
            }
        }

        private void WriteBinaryBuffer(string binaryString, BinaryWriter writer, bool isLastBlock)
        {
            int paddingSize = binaryString.Length % 8 == 0 ? 0 : 8 - binaryString.Length % 8;
            if (isLastBlock)
            {
                writer.Write(paddingSize);
                writer.Write(binaryString.Length / 8 + (paddingSize > 0 ? 1 : 0));
            }

            binaryString = binaryString.PadRight(binaryString.Length + paddingSize, '0');

            for (int i = 0; i < binaryString.Length; i += 8)
            {
                byte b = Convert.ToByte(binaryString.Substring(i, 8), 2);
                writer.Write(b);
            }
        }

        public void Dispose()
        {
            charTable.Clear();
            charTable = null;
        }


    }
}
