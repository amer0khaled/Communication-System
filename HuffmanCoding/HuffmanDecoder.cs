using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Communication_System.HuffmanCoding
{
    public class HuffmanDecoder : IDisposable
    {
        private const int BUFFER_SIZE = 8192;
        private const int MAX_CODE_LENGTH = 100;

        private string ReadBinaryData(BinaryReader reader)
        {
            int paddingSize = reader.ReadInt32();
            int byteCount = reader.ReadInt32();

            if (byteCount < 0 || byteCount > Program.MAX_FILE_SIZE)
                throw new InvalidDataException("Invalid byte count in compressed file");

            var result = new StringBuilder(byteCount * 8);
            var buffer = new byte[BUFFER_SIZE];

            int bytesRead;
            int totalBytesRead = 0;

            while ((bytesRead = reader.Read(buffer, 0, Math.Min(BUFFER_SIZE, byteCount - totalBytesRead))) > 0)
            {
                for (int i = 0; i < bytesRead; i++)
                {
                    result.Append(Convert.ToString(buffer[i], 2).PadLeft(8, '0'));
                }
                totalBytesRead += bytesRead;
            }

            if (result.Length < paddingSize)
                throw new InvalidDataException("Corrupted data: padding size larger than data");

            return result.ToString(0, result.Length - paddingSize);
        }

        private Dictionary<string, char> ReadTable(BinaryReader reader)
        {
            int tableSize = reader.ReadInt32();
            if (tableSize <= 0 || tableSize > char.MaxValue)
                throw new InvalidDataException("Invalid character table size");

            var charTable = new Dictionary<string, char>();
            for (int i = 0; i < tableSize; i++)
            {
                char data = reader.ReadChar();
                string code = reader.ReadString();

                if (code.Length > MAX_CODE_LENGTH)
                    throw new InvalidDataException($"Code length exceeds maximum of {MAX_CODE_LENGTH}");

                if (!code.All(c => c == '0' || c == '1'))
                    throw new InvalidDataException("Invalid binary code in character table");

                charTable[code] = data;
            }
            return charTable;
        }

        private void DecodeAndWriteBinaryData(string binaryData, Dictionary<string, char> charTable, string outputPath)
        {
            using var writer = new StreamWriter(outputPath);
            var currentCode = new StringBuilder();

            foreach (char bit in binaryData)
            {
                currentCode.Append(bit);
                if (currentCode.Length > MAX_CODE_LENGTH)
                    throw new InvalidDataException("Invalid encoded data: code exceeds maximum length");

                string code = currentCode.ToString();
                if (charTable.TryGetValue(code, out char decodedChar))
                {
                    writer.Write(decodedChar);
                    currentCode.Clear();
                }
            }

            if (currentCode.Length > 0)
                throw new InvalidDataException("Invalid encoded data: incomplete code at end");
        }

        public void Decode(string inputPath, string outputPath)
        {
            if (string.IsNullOrEmpty(inputPath))
                throw new ArgumentNullException(nameof(inputPath));
            if (string.IsNullOrEmpty(outputPath))
                throw new ArgumentNullException(nameof(outputPath));

            using var reader = new BinaryReader(File.Open(inputPath, FileMode.Open));
            var charTable = ReadTable(reader);
            var binaryData = ReadBinaryData(reader);
            DecodeAndWriteBinaryData(binaryData, charTable, outputPath);
        }

        public void Dispose()
        {
            // Cleanup code if needed
        }



    }
}
