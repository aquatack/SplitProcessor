using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace SplitProcessor
{
    public class TransactionFactory : IEnumerable<string>
    {
        private IEnumerable<CSVEntry> fileLines;

        public TransactionFactory(IEnumerable<CSVEntry> fileLines)
        {
            this.fileLines = fileLines;
        }

        /// <summary>
        /// Transaction Factory State Machine.
        /// This yields a new transaction on each call.
        /// </summary>
        /// <remarks>Transactions are either standard or split types. Standard ones complete on a single line,
        /// while splits run across multiple lines. This state machine iterates over the input lines returning
        /// standard or split transaction strings as appropriate. For split transactions, it will continue to 
        /// consume lines until the split transaction indicates that it is complete. At this point, the state
        /// machine will have already consumed the next line (i.e. all/part of the next transaction). The 
        /// state machine therefore has a queue that it uses to buffer splits allowing it to start building
        /// the next transaction without losing the line. The Queue is flushed at the next available opportunity
        /// before continuing to process the next transaction.</remarks>
        /// <returns>String representing the delimited representation of the transaction.</returns>
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
