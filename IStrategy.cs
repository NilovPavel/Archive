// File:    IStrategy.cs
// Author:  nilov_pg
// Created: 23 мая 2019 г. 17:16:11
// Purpose: Definition of Interface IStrategy

using System;

public interface IStrategy
{
    IReader GetIReader(string path);

    ICompress GetICompress();

}