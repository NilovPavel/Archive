// File:    DataBlock.cs
// Author:  nilov_pg
// Created: 24 мая 2019 г. 10:26:57
// Purpose: Definition of Class DataBlock

using System;
using System.IO;
using System.IO.Compression;

public class DataBlock
{
    private long id;
    private byte[] bytes;

    public long Id
    {
        get
        {
            return id;
        }
        set
        {
            this.id = value;
        }
    }

    public byte[] Bytes
    {
        get
        {
            return bytes;
        }
        set
        {
            this.bytes = value;
        }
    }

    public void CompressData()
    {
        try
        {
            using (MemoryStream compressedMemStream = new MemoryStream())
            {
                using (GZipStream compressionStream = new GZipStream(compressedMemStream, CompressionMode.Compress, true))
                {
                    (new MemoryStream(this.Bytes)).CopyTo(compressionStream);
                }
                this.Bytes = compressedMemStream.ToArray();
            }
        }
        catch (Exception exception)
        {
            Console.WriteLine(exception.Message);
        }
    }

    public void DecompressData()
    {
        try
        {
            byte[] sizeInBytes = new byte[4];
            Array.Copy(this.Bytes, this.Bytes.Length - sizeInBytes.Length, sizeInBytes, 0, sizeInBytes.Length);
            int compressSize = BitConverter.ToInt32(this.Bytes, this.Bytes.Length - 4);

            using (MemoryStream memory = new MemoryStream())
            {
                using (GZipStream stream = new GZipStream(new MemoryStream(this.Bytes), CompressionMode.Decompress))
                {
                    stream.CopyTo(memory);

                    if (memory.ToArray().Length == compressSize)
                        this.Bytes = memory.ToArray();
                    else this.Bytes = null;
                }
            }
        }
        catch (Exception exception)
        {
            Console.WriteLine(exception.Message);
        }
    }
}
