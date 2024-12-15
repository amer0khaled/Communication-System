using HuffmanCoding;

internal class Program
{
    static void Main(string[] args)
    {
        if (args.Length < 2 || (args[0] == "-d" && args.Length < 3))
        {
            Console.Error.WriteLine("Invalid CLI arguments.");
            return;
        }

        try
        {
            if (args[0] == "-d")
            {
                var decoder = new HuffmanDecoder();
                decoder.Decode(args[1], args[2]);
            }
            else
            {
                var encoder = new HuffmanEncoder(args[0], args[1]);
                encoder.Encode();
            }
        }
        catch (IOException e)
        {
            Console.Error.WriteLine(e.Message);
        }
    }
}