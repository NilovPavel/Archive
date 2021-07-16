// File:    MainClass.cs
// Author:  nilov_pg
// Created: 23 мая 2019 г. 15:56:26
// Purpose: Definition of Class MainClass

using System;

public class MainClass
{
    public static int Main(string[] args)
    {
        args = new string[] 
        {
            "compress",
            @"I:\Установка\Девушки фэнтези.exe",
            @"I:\Установка\Девушки фэнтези.exe.gzip"
        };
        GZip gZip = new GZip(args[1], args[2]);
        IStrategy strategy = default(IStrategy);
        switch (args[0])
        {
            case "compress":
                strategy = new CompressStrategy();
                break;
            case "decompress":
                strategy = new DecompressStrategy();
                break;
        }
        gZip.SetStrategy(strategy);
        gZip.Run();
        //gZip.RunThreads();
        return 0;
    }

}