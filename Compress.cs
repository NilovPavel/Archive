using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MultiThread
{

    public class MainClass
    {
        static int Main(string[] args)
        {
            try
            {
                AbstractCompressOperation operation;
                switch (args[0])
                {
                    case "compress":
                        operation = new CompressOperation(args[1], args[2]);
                        break;
                    case "decompress":
                        operation = new DecompressOperation(args[1], args[2]);
                        break;
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
                return 1;
            }

            /*new CompressOperation(@"D:\Sources\Testes\MultiThread\MultiThread\bin\Release\-23186981_200687328.mp3", @"D:\Sources\Testes\MultiThread\MultiThread\bin\Release\-23186981_200687328.mp3.gz");
            new DecompressOperation(@"D:\Sources\Testes\MultiThread\MultiThread\bin\Release\-23186981_200687328.mp3.gz", @"D:\Sources\Testes\MultiThread\MultiThread\bin\Release\_-23186981_200687328.mp3");*/
            //new DecompressOperation(@"D:\Sources\Testes\MultiThread\MultiThread\bin\Release\Debian.vdi.gz", @"D:\Sources\Testes\MultiThread\MultiThread\bin\Release\_Debian.vdi");
            //new CompressOperation(@"D:\Sources\Testes\MultiThread\MultiThread\bin\Release\Debian.vdi", @"D:\Sources\Testes\MultiThread\MultiThread\bin\Release\Debian.vdi.gz");
            return 0;
        }
    }
}
