// File:    Compress.cs
// Author:  nilov_pg
// Created: 23 мая 2019 г. 17:24:23
// Purpose: Definition of Class Compress

using System;
using System.Collections.Generic;
using System.Threading;

public class Compress : ICompress
{
    protected override DataBlock CompressedOneBlock(DataBlock dataBlock)
    {
        dataBlock.CompressData();
        return dataBlock;
    }
}