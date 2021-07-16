// File:    GZip.cs
// Author:  nilov_pg
// Created: 23 мая 2019 г. 16:47:30
// Purpose: Definition of Class GZip

using System;
using System.Collections.Concurrent;
using System.Threading;

public class GZip
{
    private int coresCount;
    private string inputPath;
    private string outputPath;
    private IReader iReader;
    private ICompress iCompress;
    private Writer writer;
    private BlockingCollection<DataBlock> dataBlocks;
    private Semaphore semaphore;

    private void Initialization()
    {
        this.coresCount = Environment.ProcessorCount;
        this.writer = new Writer(this.outputPath, this.dataBlocks);
        this.dataBlocks = new BlockingCollection<DataBlock>(this.coresCount);
        this.semaphore = new Semaphore(this.coresCount, this.coresCount);
    }

    public GZip(string inputPath, string outputPath)
    {
        this.inputPath = inputPath;
        this.outputPath = outputPath;
        this.Initialization();
    }

    public void SetStrategy(IStrategy iStrategy)
    {
        this.iReader = iStrategy.GetIReader(this.inputPath, this.dataBlocks);
        this.iCompress = iStrategy.GetICompress();
    }

    private void Worker()
    {
        semaphore.WaitOne();
        DataBlock dataBlock;
        this.dataBlocks.TryTake(out dataBlock);
        dataBlock.Bytes = this.iCompress.CompressBlock(dataBlock.Bytes);
        this.dataBlocks.TryAdd(dataBlock);
        semaphore.Release();
    }

    public void Run()
    {
        while (!this.iReader.IsEnd())
        {
            this.iReader.ReadBlocks();
            while(this.dataBlocks.Count == 0)
            { 
                Thread thread = new Thread(this.Worker);
                thread.Start();
            }
            this.writer.WriteBlocks();
        }
    }
}