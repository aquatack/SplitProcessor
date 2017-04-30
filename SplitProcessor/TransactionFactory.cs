using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace SplitProcessor
{
    public class TransactionFactory : IEnumerable<string>
    {
        private IEnumerable<CSVEntry> itemEntries;

        public TransactionFactory(IEnumerable<CSVEntry> itemEntries)
        {
            this.itemEntries = itemEntries;
        }

        public IEnumerator<string> GetEnumerator()
        {
            return GetTransactionEnumerator();
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
        public IEnumerator<string> GetTransactionEnumerator()
        {
            Queue<string> tQueue = new Queue<string>();
            Transaction currentTransaction = null;
            foreach (var itemEntry in this.itemEntries)
            {
                if (currentTransaction == null)
                    currentTransaction = GetTranaction(itemEntry);

                // If this fails, the split is complete. add to the queue.
                if (!currentTransaction.AddEntry(itemEntry))
                {
                    if (!currentTransaction.IsTransactionComplete())
                        throw new ApplicationException("A transaction is not accepting a new entry but also indicates it's not complete.");

                    // The split transaction is complete so cache it.
                    tQueue.Enqueue(currentTransaction.FullTransactionString());

                    // Now get on with the next transaction, as we've already had to pull the data to detect the end of the split.
                    currentTransaction = GetTranaction(itemEntry);
                    if (!currentTransaction.AddEntry(itemEntry))
                        throw new ApplicationException("The new transaction we're trying to add (following a split) is not accepting entries.");
                }
                while (tQueue.Any())
                {
                    yield return tQueue.Dequeue();
                }
                if (currentTransaction.IsTransactionComplete())
                {
                    yield return currentTransaction.FullTransactionString();
                    currentTransaction = null;
                }
            }
            // If a split transaction is the final thing in the list, the split won't know it's completed (as it
            // needs to see a standard line before it can complete). In this case, we just need to assume it's 
            // complete, and return it. Also, flush the queue again just in case.
            if(currentTransaction != null)
                yield return currentTransaction.FullTransactionString();

            while (tQueue.Any())
            {
                yield return tQueue.Dequeue();
            }
        }

        private Transaction GetTranaction(CSVEntry entry)
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
