// File:    GZip.cs
// Author:  nilov_pg
// Created: 23 мая 2019 г. 16:47:30
// Purpose: Definition of Class GZip

using System;
using System.Collections.Concurrent;
using System.Threading;

public class GZip
{
    private string inputPath;
    private string outputPath;
    private IStrategy iStrategy;
    private IReader iReader;
    private ICompress iCompress;
    private Writer writer;

    private void Initialization()
    {
        this.writer = new Writer(this.outputPath);
    }

    public GZip(string inputPath, string outputPath)
    {
        this.inputPath = inputPath;
        this.outputPath = outputPath;
        this.Initialization();
    }

    public void SetStrategy(IStrategy iStrategy)
    {
        this.iStrategy = iStrategy;
        this.iReader = this.iStrategy.GetIReader(this.inputPath);
        this.iCompress = this.iStrategy.GetICompress();
    }

    public void RunThreads()
    {
        Thread[] threads = new Thread[3]
            { new Thread(this.iReader.ReadBlocks),
               new Thread(this.iCompress.RunThreadsCompressedBlocks),
               new Thread(this.writer.WriteBlocks) };

        foreach (Thread thread in threads)
            thread.Start();

        foreach (Thread thread in threads)
            thread.Join();
    }

}