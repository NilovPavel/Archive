// File:    ICompress.cs
// Author:  nilov_pg
// Created: 29 мая 2019 г. 8:53:50
// Purpose: Definition of Class ICompress

using System;
using System.Collections.Concurrent;
using System.Threading;

public abstract class ICompress
{
    private int cores;

    private void Initialization()
    {
        this.cores = Environment.ProcessorCount;
    }

    protected abstract DataBlock CompressedOneBlock(DataBlock dataBlock);

    public void RunThreadsCompressedBlocks()
    {
        Thread[] threads = new Thread[this.cores];
        for (int i = 0; i < threads.Length; i++)
        {
            threads[i] = new Thread(this.CompressBlocks);
            threads[i].Start();
        }

        foreach (Thread thread in threads)
            thread.Join();

        Writer.writeCollection.CompleteAdding();
    }

    private void CompressBlocks()
    {
        try
        {
            while (!CompressReader.readCollection.IsCompleted)
            {
                DataBlock dataBlock = CompressReader.readCollection.Take();
                dataBlock = this.CompressedOneBlock(dataBlock);
                Writer.writeCollection.Add(dataBlock);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    public ICompress()
    {
        this.Initialization();
    }
}