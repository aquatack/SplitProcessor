using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SplitProcessor
{
    class StandardTransaction : Transaction
    {
        private string transactionLine;
        private bool transComplete = false;
        public override bool AddLine(string line)
        {
            if (IsStandardTransaction(line))
            {
                this.transactionLine = line;
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
            return this.transactionLine + Environment.NewLine;
        }

        public static bool IsStandardTransaction(string line)
        {
            return !SplitTransaction.IsSplitHeader(line) && !SplitTransaction.IsSplitSubEntry(line);
        }
    }
}
