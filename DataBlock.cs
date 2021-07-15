using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiThread
{
    class DataBlock : IDisposable
    {
        public long Id { get; private set; }

        public byte[] Data { get; private set; }

        public void CompressData()
        {
            try
            {
                using (MemoryStream compressedMemStream = new MemoryStream())
                {
                    using (GZipStream compressionStream = new GZipStream(compressedMemStream, CompressionMode.Compress, true))
                    {
                        (new MemoryStream(this.Data)).CopyTo(compressionStream);
                    }
                    this.Data = compressedMemStream.ToArray();
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
            }
        }

        public void DecompressData()
        {
            try
            {
                byte[] sizeInBytes = new byte[4];
                Array.Copy(this.Data, this.Data.Length - sizeInBytes.Length, sizeInBytes, 0, sizeInBytes.Length);
                int compressSize = BitConverter.ToInt32(this.Data, this.Data.Length - 4);

                using (MemoryStream memory = new MemoryStream())
                {
                    using (GZipStream stream = new GZipStream(new MemoryStream(this.Data), CompressionMode.Decompress))
                    {
                        stream.CopyTo(memory);

                        if (memory.ToArray().Length == compressSize)
                            this.Data = memory.ToArray();
                        else this.Data = null;
                    }
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
            }
        }

        public DataBlock(long readBlockCount, byte[] data)
        {
            this.Id = readBlockCount;
            this.Data = data;
        }

        void IDisposable.Dispose()
        {
            this.Data = null;
        }
    }
}
