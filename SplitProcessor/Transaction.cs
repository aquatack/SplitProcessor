using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SplitProcessor
{
    public abstract class Transaction
    {
        public abstract bool AddEntry(CSVEntry line);
        public abstract bool TransactionComplete();
        public abstract string FullTransactionString();
    }
}
