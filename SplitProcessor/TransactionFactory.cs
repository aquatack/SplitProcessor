using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace SplitProcessor
{
    public class TransactionFactory : IEnumerable<string>
    {
        private IEnumerable<CSVEntry> fileLines;
        //private IEnumerator fileLinesEnumerator;
        public TransactionFactory(IEnumerable<CSVEntry> fileLines)
        {
            this.fileLines = fileLines;
            //this.fileLinesEnumerator = fileLines.GetEnumerator();
        }

        public IEnumerator<string> GetEnumerator()
        {
            Queue<string> tQueue = new Queue<string>();
            Transaction currentTransaction = null;
            foreach (var inputLine in this.fileLines)
            {
                if (currentTransaction == null)
                    currentTransaction = GetTranaction(inputLine);

                // If this fails, the split is complete. add to the queue.
                if (!currentTransaction.AddEntry(inputLine))
                {
                    if (!currentTransaction.TransactionComplete())
                        throw new ApplicationException();

                    tQueue.Enqueue(currentTransaction.FullTransactionString());
                    currentTransaction = GetTranaction(inputLine);
                    if (!currentTransaction.AddEntry(inputLine))
                        throw new ApplicationException();
                }
                while (tQueue.Any())
                {
                    yield return tQueue.Dequeue();
                }
                if (currentTransaction.TransactionComplete())
                {
                    yield return currentTransaction.FullTransactionString();
                    currentTransaction = null;
                }
            }
        }

        private Transaction GetTranaction(CSVEntry entry)
        {
            if (StandardTransaction.IsStandardTransaction(entry))
                return new StandardTransaction();
            else if (SplitTransaction.IsSplitHeader(entry))
                return new SplitTransaction();
            else if (SplitTransaction.IsSplitSubEntry(entry))
                throw new ApplicationException();
            else
                throw new ApplicationException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
