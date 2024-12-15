# Huffman Coding

This project implements Huffman Coding, a lossless data compression algorithm. It provides both encoding and decoding functionality through a command-line interface (CLI). You can use this tool to encode a text file into its Huffman-encoded binary form and decode it back to its original form.

## Features

- **Encode**: Convert a text file into a compressed binary file using Huffman coding.
- **Decode**: Convert a Huffman-encoded binary file back into the original text.

## Requirements

- .NET 6.0 or higher.

## Usage

### Encoding

To encode a text file using Huffman coding, run the following command:

```bash
dotnet run -- "input/file/path.txt" "encoded/file/path.bin"
```
### Decoding

To decode a Huffman-encoded file back into its original text, run the following command:

```bash
dotnet run -- -d "encoded/file/path.txt" "decoded/file/path.txt"
```

### How It Works

    Encoding:
        The encoder reads the input text file, calculates the frequency of each character, and constructs a Huffman tree based on these frequencies.
        Each character is assigned a unique binary code based on its position in the tree.
        The text is then encoded into a binary string using the generated codes, and the binary data is saved to a file.

    Decoding:
        The decoder reads the encoded binary file and retrieves the Huffman character table and binary data.
        It then decodes the binary data back into the original text by traversing the Huffman tree using the character codes.

### Classes

## HuffmanEncoder

Handles the encoding of the input text file using Huffman coding. It:

    Calculates character frequencies.
    Builds the Huffman tree.
    Generates a character table with Huffman codes.
    Encodes the input text and writes the result to a binary file.

## HuffmanDecoder

Handles the decoding of a Huffman-encoded binary file. It:

    Reads the encoded binary data and the Huffman character table.
    Decodes the binary data and writes the original text to a file.

## HuffmanNode

Represents a node in the Huffman tree. It holds the frequency of a character and pointers to its left and right children.

