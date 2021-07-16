// File:    IReader.cs
// Author:  nilov_pg
// Created: 29 мая 2019 г. 9:01:02
// Purpose: Definition of Class IReader

using System;
using System.Collections.Concurrent;
using System.IO;

public abstract class IReader
{
    private string path;
    protected long lastIndex;
    protected int bufferSize;
    protected FileStream fileStream;
    public static BlockingCollection<DataBlock> readCollection;

    private void Initialization()
    {
        this.bufferSize = 1024 * 1024 * 4;
        this.fileStream = new FileStream(this.path, FileMode.Open, FileAccess.Read);
        this.lastIndex = this.fileStream.Length / this.bufferSize + (this.fileStream.Length % this.bufferSize > 0 ? 1 : 0);
        readCollection = new BlockingCollection<DataBlock>(8);
    }

    protected byte[] ReadFileBlock(long blockCount)
    {
        byte[] bytes = new byte[0];
        this.fileStream.Position = blockCount * bufferSize;
        bytes = blockCount != this.lastIndex - 1 ? new byte[this.bufferSize] : new byte[this.fileStream.Length - this.fileStream.Position];
        this.fileStream.Read(bytes, 0, bytes.Length);
        return bytes;
    }

    internal abstract bool IsEnd();
    public abstract void ReadBlocks();

    public IReader(string path)
    {
        this.path = path;
        this.Initialization();
    }
}