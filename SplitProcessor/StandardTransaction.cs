using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FileHelpers;

namespace SplitProcessor
{
    class StandardTransaction : Transaction
    {
        private CSVEntry transactionLine;
        private bool transComplete = false;
        private FileHelperEngine<CSVEntry> helperEngine = new FileHelperEngine<CSVEntry>();

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
            return this.helperEngine.WriteString(new[] { this.transactionLine });
        }

        public static bool IsStandardTransaction(CSVEntry entry)
        {
            return !SplitTransaction.IsSplitHeader(entry) && !SplitTransaction.IsSplitSubEntry(entry);
        }
    }
}
