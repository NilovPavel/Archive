using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MultiThread
{
    abstract class AbstractCompressOperation
    {
        private string sourceFileName;
        private string destinationFileName;
        protected BlockingCollection<DataBlock> readCollection;
        protected BlockingCollection<DataBlock> writeCollection;
        protected int bufferSize;
        protected long lastIndex;
        protected int cors;

        private void Initialization()
        {
            this.cors = Environment.ProcessorCount;
            this.bufferSize = 1024 * 1024 * 4;
            this.readCollection = new BlockingCollection<DataBlock>(this.cors);
            this.writeCollection = new BlockingCollection<DataBlock>(this.cors);
            using (FileStream outputFileStream = new FileStream(this.destinationFileName, FileMode.Create));
            using (FileStream inputFileStream = new FileStream(this.sourceFileName, FileMode.Open, FileAccess.Read))
            {
                this.lastIndex = inputFileStream.Length / this.bufferSize + (inputFileStream.Length % this.bufferSize > 0 ? 1 : 0);
            }
            this.SetPrivateSettings();
        }

        abstract protected void SetPrivateSettings();

        public AbstractCompressOperation(string sourceFileName, string destinationFileName)
        {
            this.sourceFileName = sourceFileName;
            this.destinationFileName = destinationFileName;
            this.Initialization();
            new Thread(this.ReadBlocks).Start();
            new Thread(this.GZipOperations).Start();
            new Thread(this.WriteBlocks).Start();
        }

        private void ReadBlocks()
        {
            for (long readBlockCount = 0; readBlockCount < this.lastIndex; readBlockCount++)
            {
                DataBlock dataBlock = new DataBlock(readBlockCount, this.ReadOneBlock(readBlockCount));
                this.readCollection.Add(dataBlock);
            }
            this.readCollection.CompleteAdding();
        }

        protected DataBlock GetDataBlockFromCollections(ref ConcurrentDictionary<long, DataBlock> cashe, long blockCount)
        {
            DataBlock blockFromCollection, blockFromCashe;
            this.writeCollection.TryTake(out blockFromCollection);
            if (blockFromCollection != null)
                cashe.TryAdd(blockFromCollection.Id, blockFromCollection);
            cashe.TryRemove(blockCount, out blockFromCashe);
            return blockFromCashe;
        }

        protected void RunThreads()
        {
            Thread[] threads = new Thread[this.cors];
            for (int i = 0; i < threads.Length; i++)
            {
                threads[i] = new Thread(this.OperationsWithBlocks);
                threads[i].Start();
            }

            foreach (Thread thread in threads)
                thread.Join();
        }

        abstract protected void GZipOperations();

        abstract protected void OperationsWithBlocks();

        abstract protected void WriteBlocks();

        private byte[] ReadOneBlock(long readBlockCount)
        {
            byte[] bytes = new byte[0];
            try
            {

                using (FileStream inputFileStream = new FileStream(this.sourceFileName, FileMode.Open, FileAccess.Read))
                {
                    inputFileStream.Position = readBlockCount * bufferSize;
                    bytes = readBlockCount != this.lastIndex - 1 ? new byte[this.bufferSize] : new byte[inputFileStream.Length - inputFileStream.Position];
                    inputFileStream.Read(bytes, 0, bytes.Length);
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
            }

            return bytes;
        }

        protected void WriteOneBlock(DataBlock dataBlock)
        {
            try
            {
                using (FileStream fileStream = new FileStream(this.destinationFileName, FileMode.Append))
                {
                    fileStream.Write(dataBlock.Data, 0, dataBlock.Data.Length);
                    
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
            }
            finally
            {
                ((IDisposable)dataBlock).Dispose();
            }
        }
    }
}
