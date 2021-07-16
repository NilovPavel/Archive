// File:    CompressReader.cs
// Author:  nilov_pg
// Created: 23 мая 2019 г. 17:28:43
// Purpose: Definition of Class CompressReader

using System;
using System.Collections.Concurrent;

public class CompressReader : IReader
{
    private BlockingCollection<DataBlock> dataBlocks;
    private long currentIndex;
    public override void ReadBlocks()
    {
        for (long readBlockCount = currentIndex; !this.dataBlocks.IsAddingCompleted; readBlockCount++)
        {
            DataBlock dataBlock = new DataBlock
            {
                Id = readBlockCount,
                Bytes = this.ReadFileBlock(readBlockCount)
            };

            if (!this.dataBlocks.TryAdd(dataBlock))
            {
                currentIndex = readBlockCount;
                break;
            }
        }
        
        if(currentIndex != this.lastIndex - 1)
            return;

        this.dataBlocks.CompleteAdding();
        this.fileStream.Close();
        this.fileStream.Dispose();
    }

    internal override bool IsEnd()
    {
        return this.dataBlocks.IsCompleted; 
    }

    public CompressReader(string path, BlockingCollection<DataBlock> dataBlocks) : base(path)
    {
        this.dataBlocks = dataBlocks;
        this.currentIndex = 0;
    }
}