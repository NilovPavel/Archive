// File:    Writer.cs
// Author:  nilov_pg
// Created: 29 мая 2019 г. 8:59:07
// Purpose: Definition of Class Writer

using System;
using System.Collections.Concurrent;
using System.IO;

public class Writer
{
    private string path;
    private FileStream fileStream;
    public static BlockingCollection<DataBlock> writeCollection;

    private bool WriteOneBlock(DataBlock dataBlock)
    {
        if (dataBlock == null)
            return false;
        this.fileStream.Write(dataBlock.Bytes, 0, dataBlock.Bytes.Length);
        return true;
    }

    private void Initialization()
    {
        using (FileStream newFilestream = new FileStream(this.path, FileMode.Create, FileAccess.Write)) { }
        this.fileStream = new FileStream(this.path, FileMode.Append, FileAccess.Write);
        writeCollection = new BlockingCollection<DataBlock>(8);
    }

    public Writer(string path, BlockingCollection<DataBlock> dataBlocks)
    {
        this.path = path;
        this.Initialization();
    }

    private DataBlock GetDataBlockFromCollections(ref ConcurrentDictionary<long, DataBlock> cashe, long blockCount)
    {
        DataBlock blockFromCollection, blockFromCashe;
        writeCollection.TryTake(out blockFromCollection);
        if (blockFromCollection != null)
            cashe.TryAdd(blockFromCollection.Id, blockFromCollection);
        cashe.TryRemove(blockCount, out blockFromCashe);
        return blockFromCashe;
    }

    public void WriteBlocks()
    {
        ConcurrentDictionary<long, DataBlock> cashe = new ConcurrentDictionary<long, DataBlock>();
        for (int i = 0; !(writeCollection.IsCompleted && cashe.IsEmpty); i++)
        {
            DataBlock dataBlock = this.GetDataBlockFromCollections(ref cashe, i);
            if (!this.WriteOneBlock(dataBlock))
                i--;
        }

        this.fileStream.Close();
        this.fileStream.Dispose();
    }
}