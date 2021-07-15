// File:    CompressStrategy.cs
// Author:  nilov_pg
// Created: 23 мая 2019 г. 17:21:04
// Purpose: Definition of Class CompressStrategy

using System;

public class CompressStrategy : IStrategy
{
    public CompressStrategy()
    { }

    public IReader GetIReader(string path)
    {
        return new CompressReader(path);
    }

    public ICompress GetICompress()
    {
        return new Compress();
    }

}
