// File:    CompressReader.cs
// Author:  nilov_pg
// Created: 23 мая 2019 г. 17:28:43
// Purpose: Definition of Class CompressReader

using System;

public class CompressReader : IReader
{
    public override void ReadBlocks()
    {
        for (long readBlockCount = 0; readBlockCount < this.lastIndex; readBlockCount++)
        {
            DataBlock dataBlock = new DataBlock
            {
                Id = readBlockCount,
                Bytes = this.ReadFileBlock(readBlockCount)
            };
            readCollection.Add(dataBlock);
        }
        readCollection.CompleteAdding();
        this.fileStream.Close();
        this.fileStream.Dispose();
    }

    public CompressReader(string path) : base(path)
    {
    }
}