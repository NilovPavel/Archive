// File:    DecompressReader.cs
// Author:  nilov_pg
// Created: 24 мая 2019 г. 10:23:55
// Purpose: Definition of Class DecompressReader

using System;
using System.Collections.Generic;
using System.Linq;

public class DecompressReader : IReader
{
    private byte[] footerBlock;
    private List<byte> currentBytes;
    private long dataBlockCount;

    public DecompressReader(string path) : base(path)
    {
        this.footerBlock = BitConverter.GetBytes(this.bufferSize);
        this.currentBytes = new List<byte>();
    }

    private int SearchBytes(byte[] bytes)
    {
        for (int i = 0; i < bytes.Length; i++)
        {
            if (i + 3 >= bytes.Length)
                return -1;

            if (bytes[i] == 31 && bytes[i + 1] == 139 && bytes[i + 2] == 8)
            {
                if (i == 0)
                    continue;
                else if (bytes[i - 4] == this.footerBlock[0] && bytes[i - 3] == this.footerBlock[1] && bytes[i - 2] == this.footerBlock[2] && bytes[i - 1] == this.footerBlock[3])
                    return i;
            }
        }
        return -1;
    }

    private byte[] GetGoodBlocksAndRemainder(byte[] currentCollection)
    {
        byte[] bytesRemainder = currentCollection;
        int searchIndex = 0;
        while ((searchIndex = this.SearchBytes(currentCollection.ToArray())) != -1)
        {
            byte[] array = new byte[searchIndex];
            Array.Copy(currentCollection, 0, array, 0, array.Length);
            bytesRemainder = new byte[currentCollection.Length - searchIndex];
            Array.Copy(currentCollection, searchIndex, bytesRemainder, 0, bytesRemainder.Length);
            currentCollection = bytesRemainder;
            readCollection.Add(new DataBlock { Id = dataBlockCount, Bytes = array });
            this.dataBlockCount++;
        }

        return bytesRemainder;
    }


    public override void ReadBlocks()
    {
        for (long readBlockCount = 0; readBlockCount < this.lastIndex; readBlockCount++)
        {
            this.currentBytes.AddRange(this.ReadFileBlock(readBlockCount));
            byte[] remainder = this.GetGoodBlocksAndRemainder(currentBytes.ToArray());
            this.currentBytes.Clear();
            this.currentBytes.AddRange(remainder);
        }
        readCollection.Add(new DataBlock { Id = this.dataBlockCount, Bytes = this.currentBytes.ToArray() });
        readCollection.CompleteAdding();
        this.fileStream.Close();
        this.fileStream.Dispose();
    }
}