// File:    DecompressStrategy.cs
// Author:  nilov_pg
// Created: 23 мая 2019 г. 17:21:05
// Purpose: Definition of Class DecompressStrategy

using System;
using System.Collections.Concurrent;

public class DecompressStrategy : IStrategy
{
    public DecompressStrategy()
    { }

    public IReader GetIReader(string path)
    {
        return new DecompressReader(path);
    }

    public ICompress GetICompress()
    {
        return new Decompress();
    }

    public IReader GetIReader(string path, BlockingCollection<DataBlock> dataBlocks)
    {
        throw new NotImplementedException();
    }
}
