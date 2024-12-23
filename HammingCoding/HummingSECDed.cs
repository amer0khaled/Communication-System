using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Communication_System.HammingCoding
{
    // Single Error Correction, Double Error Detection
    public class HummingSECDed
    {

        private const int BITS_PER_BYTE = 8;
        private const int MAX_DATA_LENGTH = 1024 * 1024 * 8; // 1MB in bits

        public static BitArray Encode(BitArray data)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));
            if (data.Length > MAX_DATA_LENGTH)
                throw new ArgumentException($"Data length exceeds maximum of {MAX_DATA_LENGTH} bits");
            if (data.Length == 0)
                throw new ArgumentException("Data length cannot be zero");

            int dataLength = data.Length;
            int numParityBits = NumParityBitsNeeded(dataLength);
            int encodedLength = dataLength + numParityBits + 1;

            var encoded = new BitArray(encodedLength);

            try
            {
                // Set Parity Bits
                foreach (int parityIndex in PowersOfTwo(numParityBits))
                {
                    encoded[parityIndex] = CalculateParity(data, parityIndex);
                }

                // Set Data Bits
                int dataIndex = 0;
                for (int encodedIndex = 3; encodedIndex < encoded.Length; encodedIndex++)
                {
                    if (!IsPowerOfTwo(encodedIndex))
                    {
                        encoded[encodedIndex] = data[dataIndex++];
                    }
                }

                // Overall Parity
                encoded[0] = CalculateParity(encoded, 0);

                return encoded;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Error during Hamming encoding", ex);
            }
        }

        public static BitArray Decode(BitArray encoded)
        {
            if (encoded == null)
                throw new ArgumentNullException(nameof(encoded));
            if (encoded.Length == 0)
                throw new ArgumentException("Encoded data length cannot be zero");

            try
            {
                int encodedLength = encoded.Length;
                int numParityBits = (int)Math.Floor(Math.Log2(encodedLength - 1)) + 1;
                int indexOfError = 0;

                // Check Overall Parity
                bool overallExpected = CalculateParity(encoded, 0);
                bool overallActual = encoded[0];
                bool overallCorrect = overallExpected == overallActual;

                // Check Individual Parities
                foreach (int parityIndex in PowersOfTwo(numParityBits))
                {
                    bool expected = CalculateParity(ExtractData(encoded), parityIndex);
                    bool actual = encoded[parityIndex];
                    if (expected != actual)
                    {
                        indexOfError += parityIndex;
                    }
                }

                if (indexOfError > encodedLength)
                    throw new InvalidDataException("Invalid error index detected");

                // Error Handling
                if (indexOfError > 0 && overallCorrect)
                    throw new InvalidDataException("Two errors detected - unable to correct");
                else if (indexOfError > 0 && !overallCorrect)
                    encoded[indexOfError] = !encoded[indexOfError];

                return ExtractData(encoded);
            }
            catch (Exception ex) when (!(ex is InvalidDataException))
            {
                throw new InvalidOperationException("Error during Hamming decoding", ex);
            }
        }

        // Helper methods: PowersOfTwo, IsPowerOfTwo, ExtractData, NumParityBitsNeeded, CalculateParity, etc.

        // HELPER FUNCTIONS
        private static int NumParityBitsNeeded(int length)
        {
            int n = NextPowerOfTwo(length);
            int lowerBin = (int)Math.Floor(Math.Log2(n));
            int upperBin = lowerBin + 1;
            int boundary = n - lowerBin - 1;

            return length <= boundary ? lowerBin : upperBin;
        }

        private static bool CalculateParity(BitArray data, int parity)
        {
            bool result = false;

            if (parity == 0)
            {
                foreach (bool bit in data)
                    result ^= bit;
            }
            else
            {
                foreach (int index in DataBitsCovered(parity, data.Length))
                    result ^= data[index];
            }

            return result;
        }

        private static IEnumerable<int> DataBitsCovered(int parity, int limit)
        {
            if (!IsPowerOfTwo(parity))
                throw new ArgumentException("Parity index must be a power of two.");

            int dataIndex = 1, totalIndex = 3;

            while (dataIndex <= limit)
            {
                bool isDataBit = !IsPowerOfTwo(totalIndex);
                if (isDataBit && totalIndex % (parity << 1) >= parity)
                    yield return dataIndex - 1;

                dataIndex += isDataBit ? 1 : 0;
                totalIndex++;
            }
        }

        private static BitArray ExtractData(BitArray encoded)
        {
            List<bool> dataBits = new List<bool>();

            for (int i = 3; i < encoded.Length; i++)
            {
                if (!IsPowerOfTwo(i))
                    dataBits.Add(encoded[i]);
            }

            return new BitArray(dataBits.ToArray());
        }

        private static int NextPowerOfTwo(int n)
        {
            if (n <= 0)
                throw new ArgumentException("Value must be positive.");
            if (IsPowerOfTwo(n))
                return n << 1;

            return (int)Math.Pow(2, Math.Ceiling(Math.Log2(n)));
        }

        private static bool IsPowerOfTwo(int n)
        {
            return n > 0 && (n & n - 1) == 0;
        }

        private static IEnumerable<int> PowersOfTwo(int n)
        {
            int power = 1;
            for (int i = 0; i < n; i++)
            {
                yield return power;
                power <<= 1;
            }
        }

        // Utility Methods
        public static BitArray BytesToBits(byte[] bytes)
        {
            List<bool> bits = new List<bool>();

            foreach (byte b in bytes)
            {
                for (int i = BITS_PER_BYTE - 1; i >= 0; i--)
                {
                    bits.Add((b & 1 << i) != 0);
                }
            }

            return new BitArray(bits.ToArray());
        }

        public static byte[] BitsToBytes(BitArray bits)
        {
            List<byte> bytes = new List<byte>();
            int bitIndex = 0;

            while (bitIndex < bits.Length)
            {
                byte currentByte = 0;

                for (int i = 7; i >= 0; i--)
                {
                    if (bitIndex < bits.Length)
                    {
                        currentByte |= (byte)((bits[bitIndex++] ? 1 : 0) << i);
                    }
                }

                bytes.Add(currentByte);
            }

            return bytes.ToArray();
        }




    }
}
