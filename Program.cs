
using Communication_System.HammingCoding;
using Communication_System.HuffmanCoding;

internal class Program
{
    public const int MAX_FILE_SIZE = 100 * 1024 * 1024; // 100MB

    public static void Main(string[] args)
    {
        try
        {
            ValidateArguments(args);
            ProcessFile(args);
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error: {ex.Message}");
            Environment.Exit(1);
        }
    }

    private static void ValidateArguments(string[] args)
    {
        if (args == null || args.Length < 2 || (args[0] == "-d" && args.Length < 3))
        {
            PrintUsage();
            throw new ArgumentException("Invalid arguments");
        }
    }

    private static void PrintUsage()
    {
        Console.Error.WriteLine("Usage:");
        Console.Error.WriteLine("  To Compress: HuffmanCoding <inputFilePath> <outputFilePath>");
        Console.Error.WriteLine("  To Decompress: HuffmanCoding -d <compressedFilePath> <outputFilePath>");
    }

    private static void ProcessFile(string[] args)
    {
        string tempFilePath = null;

        try
        {
            if (args[0] == "-d")
            {
                DecompressFile(args[1], args[2], ref tempFilePath);
            }
            else
            {
                CompressFile(args[0], args[1]);
            }
        }
        finally
        {
            if (tempFilePath != null && File.Exists(tempFilePath))
            {
                try
                {
                    File.Delete(tempFilePath);
                }
                catch (IOException ex)
                {
                    Console.Error.WriteLine($"Warning: Could not delete temporary file: {ex.Message}");
                }
            }
        }



    }

    private static void CompressFile(string inputPath, string outputPath)
    {
        Console.WriteLine("Compressing...");

        using var encoder = new HuffmanEncoder(inputPath, outputPath);
        encoder.Encode();

        // Read Huffman encoded data (bytes)
        byte[] huffmanEncodedBytes = File.ReadAllBytes(outputPath);

        // Convert Huffman encoded bytes to a BitArray (bits)
        var huffmanBits = HummingSECDed.BytesToBits(huffmanEncodedBytes);

        // Apply Hamming encoding (SEC-DED)
        var hammingEncodedBits = HummingSECDed.Encode(huffmanBits);

        // Convert Hamming encoded bits back to bytes
        byte[] hammingEncodedBytesFinal = HummingSECDed.BitsToBytes(hammingEncodedBits);

        // Write the final encoded data (with Hamming SEC-DED applied) to the output file
        File.WriteAllBytes(outputPath, hammingEncodedBytesFinal);

        Console.WriteLine("Compression complete.");
    }

    private static void DecompressFile(string inputPath, string outputPath, ref string tempFilePath)
    {
        Console.WriteLine("Decompressing...");

        // First, decode the Hamming SEC-DED encoded data
        byte[] hammingEncodedBytes = File.ReadAllBytes(inputPath);
        var hammingBits = HummingSECDed.BytesToBits(hammingEncodedBytes);
        var hammingDecodedBits = HummingSECDed.Decode(hammingBits);

        // Convert the decoded bits back to bytes
        byte[] huffmanEncodedBytes = HummingSECDed.BitsToBytes(hammingDecodedBits);

        // Write the intermediate Huffman encoded data to a temporary file
        tempFilePath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
        File.WriteAllBytes(tempFilePath, huffmanEncodedBytes);

        // Decode the Huffman encoded data
        using var decoder = new HuffmanDecoder();
        decoder.Decode(tempFilePath, outputPath);

        Console.WriteLine("Decompression complete.");
    }


}