// File:    Decompress.cs
// Author:  nilov_pg
// Created: 23 мая 2019 г. 17:25:08
// Purpose: Definition of Class Decompress

using System;

public class Decompress : ICompress
{
    protected override DataBlock CompressedOneBlock(DataBlock dataBlock)
    {
        dataBlock.DecompressData();
        return dataBlock;
    }
}
