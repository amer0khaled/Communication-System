# Communication System - Huffman & Hamming Coding

This project implements Huffman Encoding and Decoding along with Hamming Single Error Correction and Double Error Detection (SEC-DED). The system allows you to compress and decompress files using Huffman coding, with additional error detection and correction using Hamming codes.

## Features
- **Huffman Encoding**: Compresses text data using the Huffman coding algorithm.
- **Huffman Decoding**: Decompresses the Huffman-encoded file back to the original text.
- **Hamming SEC-DED**: Encodes and decodes the data with Hamming code to detect and correct single-bit errors and detect double-bit errors.

## File Structure
The project contains the following key components:
- **HuffmanEncoder**: Handles the Huffman encoding process, including file validation, tree building, and encoding text.
- **HuffmanDecoder**: Handles the Huffman decoding process, including reading the encoded file, decoding the Huffman data, and writing the decoded text.
- **HammingSECDed**: Provides methods to encode and decode data with Hamming SEC-DED for error detection and correction.
- **Program**: The main entry point of the program, where files are processed based on user input.

## Requirements
- .NET 6.0 or later
  
## Usage

### Command-line Usage
You can run the program from the command line to either compress or decompress a file.

- **To Compress a File**:
  ```bash
  dotnet run -- <inputFilePath> <encodedFilePath>
  ```
  
- **To Compress a File**:
  ```bash
  dotnet run -- -d <encodedFilePath> <outputFilePath>
  ```
  
## File Size Limitations

    Maximum Input File Size: 100MB
    Maximum Data Length for Hamming Encoding: 1MB (in bits)

## Error Handling

The program performs the following validations:

    The input file must exist.
    The input file must not exceed the size limit of 100MB.
    The Hamming encoding/decoding process includes error detection and correction for single-bit errors, and detection for double-bit errors.

  
