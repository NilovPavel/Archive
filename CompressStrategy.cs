// File:    CompressStrategy.cs
// Author:  nilov_pg
// Created: 23 мая 2019 г. 17:21:04
// Purpose: Definition of Class CompressStrategy

using System;
using System.Collections.Concurrent;

public class CompressStrategy : IStrategy
{
    private string path;
    private BlockingCollection<DataBlock> dataBlocks;

    public CompressStrategy()
    { }

    /*public IReader GetIReader(string path)
    {
        return new CompressReader(this.path);
    }*/

    public ICompress GetICompress()
    {
        return new Compress();
    }

    public IReader GetIReader(string path, BlockingCollection<DataBlock> dataBlocks)
    {
        /*this.path = path;
        this.dataBlocks = dataBlocks;*/
        return new CompressReader(path, dataBlocks);
    }
}
