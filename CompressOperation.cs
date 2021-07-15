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
    class CompressOperation : AbstractCompressOperation
    {
        public CompressOperation(string sourceFileName, string destinationFileName) : base(sourceFileName, destinationFileName)
        {
        }

        protected override void WriteBlocks()
        {
            ConcurrentDictionary<long, DataBlock> cashe = new ConcurrentDictionary<long, DataBlock>();
            for (long readBlockCount = 0; readBlockCount < this.lastIndex; readBlockCount++)
            {
                DataBlock blockFromCashe = this.GetDataBlockFromCollections(ref cashe, readBlockCount);

                if (blockFromCashe == null)
                {
                    readBlockCount--;
                    continue;
                }
                this.WriteOneBlock(blockFromCashe);
            }
        }

        protected override void GZipOperations()
        {
            this.RunThreads();
        }

        protected override void OperationsWithBlocks()
        {
            foreach (DataBlock dataBlock in this.readCollection.GetConsumingEnumerable())
            {
                dataBlock.CompressData();
                this.writeCollection.Add(dataBlock);
            }
        }

        protected override void SetPrivateSettings()
        {
        }
    }
}
