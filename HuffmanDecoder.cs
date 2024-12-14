using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Communication_System
{
    public class HuffmanDecoder
    {
        private string ReadBinaryData(BinaryReader reader)
        {
            int paddingSize = reader.ReadInt32();
            int byteCount = reader.ReadInt32();
            var binaryData = "";

            for (int i = 0; i < byteCount; i++)
            {
                byte b = reader.ReadByte();
                binaryData += Convert.ToString(b, 2).PadLeft(8, '0');
            }

            return binaryData.Substring(0, binaryData.Length - paddingSize);
        }

        private Dictionary<string, char> ReadTable(BinaryReader reader)
        {
            int tableSize = reader.ReadInt32();
            var charTable = new Dictionary<string, char>();

            for (int i = 0; i < tableSize; i++)
            {
                char data = reader.ReadChar();
                reader.ReadInt32();
                string code = reader.ReadString();
                charTable[code] = data;
            }

            return charTable;
        }

        private void DecodeAndWriteBinaryData(string binaryData, Dictionary<string, char> charTable, string outputPath)
        {
            using var writer = new StreamWriter(outputPath);
            string currentCode = "";

            foreach (char bit in binaryData)
            {
                currentCode += bit;
                if (charTable.ContainsKey(currentCode))
                {
                    writer.Write(charTable[currentCode]);
                    currentCode = "";
                }
            }
        }

        public void Decode(string inputPath, string outputPath)
        {
            using var reader = new BinaryReader(File.Open(inputPath, FileMode.Open));
            var charTable = ReadTable(reader);
            var binaryData = ReadBinaryData(reader);
            DecodeAndWriteBinaryData(binaryData, charTable, outputPath);
        }
    }

}
