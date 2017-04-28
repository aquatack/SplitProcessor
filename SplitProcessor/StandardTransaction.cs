using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SplitProcessor
{
    class StandardTransaction : Transaction
    {
        private CSVEntry transactionLine;
        private bool transComplete = false;
        public override bool AddEntry(CSVEntry entry)
        {
            if (IsStandardTransaction(entry))
            {
                this.transactionLine = entry;
                this.transComplete = true;
                return true;
            }
            return false;
        }

        public override bool TransactionComplete()
        {
            return true;
        }

        public override string FullTransactionString()
        {
            return this.transactionLine.ToDelimitedString() + Environment.NewLine;
        }

        public static bool IsStandardTransaction(CSVEntry entry)
        {
            return !SplitTransaction.IsSplitHeader(entry) && !SplitTransaction.IsSplitSubEntry(entry);
        }
    }
}
