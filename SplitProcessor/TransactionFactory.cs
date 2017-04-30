using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace SplitProcessor
{
    public class TransactionFactory : IEnumerable<string>
    {
        private IEnumerable<CSVEntry> inputEntries;

        public TransactionFactory(IEnumerable<CSVEntry> fileLines)
        {
            this.inputEntries = fileLines;
        }

        public IEnumerator<string> GetEnumerator()
        {
            return GetSimpleTransactionEnumerator();
        }

        private IEnumerator<string> GetSimpleTransactionEnumerator()
        {
            Transaction transaction = null;
            foreach(var inputEntry in this.inputEntries)
            {
                if(transaction == null)
                    transaction = GetTransaction(inputEntry);

                if (!transaction.AddEntry(inputEntry))
                    throw new ApplicationException("Adding an entry has failed.");

                if(transaction.IsTransactionComplete)
                {
                    yield return transaction.FullTransactionString();
                    transaction = null;
                }                
            }
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
        private IEnumerator<string> GetTransactionEnumerator()
        {
            Queue<string> tQueue = new Queue<string>();
            Transaction currentTransaction = null;
            foreach (var inputEntry in this.inputEntries)
            {
                if (currentTransaction == null)
                    currentTransaction = GetTransaction(inputEntry);

                // If this fails, the split is complete. add to the queue.
                if (!currentTransaction.AddEntry(inputEntry))
                {
                    if (!currentTransaction.IsTransactionComplete)
                        throw new ApplicationException();

                    tQueue.Enqueue(currentTransaction.FullTransactionString());
                    currentTransaction = GetTransaction(inputEntry);
                    if (!currentTransaction.AddEntry(inputEntry))
                        throw new ApplicationException();
                }
                while (tQueue.Any())
                {
                    yield return tQueue.Dequeue();
                }
                if (currentTransaction.IsTransactionComplete)
                {
                    yield return currentTransaction.FullTransactionString();
                    currentTransaction = null;
                }
            }
            // If a split transaction is the final thing in the list, the split won't know it's completed (as it
            // needs to see a standard line before it can complete). In this case, we just need to assume it's 
            // complete, and return it. Also, flush the queue again just in case.
            yield return currentTransaction.FullTransactionString();
            while(tQueue.Any())
            {
                yield return tQueue.Dequeue();
            }
        }

        private Transaction GetTransaction(CSVEntry entry)
        {
            if (StandardTransaction.IsStandardTransaction(entry))
                return new StandardTransaction();
            else if (SplitTransaction.IsSplitHeader(entry))
                return new SplitTransaction();
            else if (SplitTransaction.IsSplitSubEntry(entry))
                throw new ApplicationException("Attempting to add a split sub-entry before a split header has appeared");
            else
                throw new ApplicationException("An unidentified an entry is being added (neither a standard, split header nor split body transaction entry).");
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
