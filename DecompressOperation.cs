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
    class DecompressOperation : AbstractCompressOperation
    {
        private BlockingCollection<DataBlock> clearenceCollecttion;
        private long clearenceCollectionCount;
        private byte[] footerBlock;

        public DecompressOperation(string sourceFileName, string destinationFileName) : base(sourceFileName, destinationFileName)
        {
            new Thread(this.OperationOverReadsBlocks).Start();
        }

        private int SearchBytes(byte[] bytes)
        {
            for (int i = 0; i < bytes.Length; i++)
            {
                if (i + 3 >= bytes.Length)
                    return -1;

                if (bytes[i] == 31 && bytes[i + 1] == 139 && bytes[i + 2] == 8)
                {
                    if (i == 0)
                        continue;
                    else if (bytes[i - 4] == this.footerBlock[0] && bytes[i - 3] == this.footerBlock[1] && bytes[i - 2] == this.footerBlock[2] && bytes[i - 1] == this.footerBlock[3])
                        return i;
                }
            }
            return -1;
        }

        protected override void WriteBlocks()
        {
            ConcurrentDictionary<long, DataBlock> cashe = new ConcurrentDictionary<long, DataBlock>();
            for (long currentCount = 0; this.writeCollection.Count > 0 || !this.writeCollection.IsAddingCompleted || cashe.Count > 0; currentCount++) 
            {
                DataBlock blockFromCashe = this.GetDataBlockFromCollections(ref cashe, currentCount);

                if (blockFromCashe == null)
                {
                    currentCount--;
                    continue;
                }
                this.WriteOneBlock(blockFromCashe);
            }
        }

        
        private byte[] GetGoodBlocksAndRemainder(byte[] currentCollection)
        {
            byte[] bytesRemainder = currentCollection;
            int searchIndex = 0;
            while ((searchIndex = this.SearchBytes(currentCollection.ToArray())) != -1)
            {
                byte[] array = new byte[searchIndex];
                Array.Copy(currentCollection, 0, array, 0, array.Length);
                bytesRemainder = new byte[currentCollection.Length - searchIndex];
                Array.Copy(currentCollection, searchIndex, bytesRemainder, 0, bytesRemainder.Length);
                currentCollection = bytesRemainder;
                this.clearenceCollecttion.Add(new DataBlock(this.clearenceCollectionCount, array));
                this.clearenceCollectionCount++;
            }

            return bytesRemainder;
        }

        private void OperationOverReadsBlocks()
        {
            List<byte> clearenceBytes = new List<byte>();
            foreach (DataBlock dataBlock in this.readCollection.GetConsumingEnumerable())
            {
                clearenceBytes.AddRange(dataBlock.Data);
                byte[] remainder = this.GetGoodBlocksAndRemainder(clearenceBytes.ToArray());
                clearenceBytes.Clear();
                clearenceBytes.AddRange(remainder);
            }
            this.clearenceCollecttion.Add(new DataBlock(this.clearenceCollectionCount, clearenceBytes.ToArray()));
            this.clearenceCollecttion.CompleteAdding();
        }

        protected override void GZipOperations()
        {
            this.RunThreads();

            if (this.clearenceCollecttion.IsAddingCompleted && this.readCollection.IsAddingCompleted)
                this.writeCollection.CompleteAdding();
        }

        protected override void OperationsWithBlocks()
        {
            foreach (DataBlock dataBlock in this.clearenceCollecttion.GetConsumingEnumerable())
            {
                dataBlock.DecompressData();
                this.writeCollection.Add(dataBlock);
            }
            
        }

        protected override void SetPrivateSettings()
        {
            this.clearenceCollecttion = new BlockingCollection<DataBlock>(this.cors);
            this.clearenceCollectionCount = 0;
            this.footerBlock = BitConverter.GetBytes(this.bufferSize);
        }
    }
}
