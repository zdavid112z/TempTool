using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using Zlib;

public class Compression
{
    public static byte[] Inflate(byte[] compressed)
    {
        int bufferSize = 1024 * 64;
        byte[] buffer = new byte[bufferSize];
        ZlibCodec decompressor = new ZlibCodec();

        MemoryStream ms = new MemoryStream();
        
        int rc = decompressor.InitializeInflate();
        if (rc != ZlibConstants.Z_OK)
            throw new Exception("init inflate: " + decompressor.Message);
        
        decompressor.InputBuffer = compressed;
        decompressor.NextIn = 0;
        decompressor.AvailableBytesIn = compressed.Length;

        decompressor.OutputBuffer = buffer;

        // pass 1: inflate 
        do
        {
            decompressor.NextOut = 0;
            decompressor.AvailableBytesOut = bufferSize;
            rc = decompressor.Inflate(FlushType.None);
        
            if (rc != ZlibConstants.Z_OK && rc != ZlibConstants.Z_STREAM_END)
                throw new Exception("inflating: " + decompressor.Message);
        
            ms.Write(decompressor.OutputBuffer, 0, bufferSize - decompressor.AvailableBytesOut);
        }
        while (decompressor.AvailableBytesIn > 0 || decompressor.AvailableBytesOut == 0);

        // pass 2: finish and flush
        do
        {
            decompressor.NextOut = 0;
            decompressor.AvailableBytesOut = bufferSize;
            rc = decompressor.Inflate(FlushType.Finish);
        
            if (rc != ZlibConstants.Z_STREAM_END && rc != ZlibConstants.Z_OK)
                throw new Exception("inflating: " + decompressor.Message);
        
            if (bufferSize - decompressor.AvailableBytesOut > 0)
                ms.Write(decompressor.OutputBuffer, 0, bufferSize - decompressor.AvailableBytesOut);
        } while (decompressor.AvailableBytesIn > 0 || decompressor.AvailableBytesOut == 0);
        
        decompressor.EndInflate();
        return ms.ToArray();
    }

    public static byte[] Deflate(byte[] decompressed)
    {
        int rc;
        ZlibCodec compressor = new ZlibCodec();
        byte[] compressed = new byte[decompressed.Length * 2 + 16];

        rc = compressor.InitializeDeflate(Zlib.CompressionLevel.BestCompression);
        if (rc != ZlibConstants.Z_OK)
            throw new Exception("inflating: " + compressor.Message);

        compressor.InputBuffer = decompressed;
        compressor.NextIn = 0;
        compressor.AvailableBytesIn = decompressed.Length;
        
        compressor.OutputBuffer = compressed;
        compressor.NextOut = 0;
        compressor.AvailableBytesOut = compressed.Length;
        
        while (compressor.TotalBytesIn != decompressed.Length && compressor.TotalBytesOut < compressed.Length)
            compressor.Deflate(FlushType.None);

        do
        {
            rc = compressor.Deflate(FlushType.Finish);
        } while (rc != ZlibConstants.Z_STREAM_END);

        long compressedLen = compressor.TotalBytesOut;

        compressor.EndDeflate();

        byte[] result = new byte[compressedLen];
        Array.Copy(compressed, result, compressedLen);
        return result;
    }
}
