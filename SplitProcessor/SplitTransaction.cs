using System;
using System.Collections.Generic;
using System.Text;

using FileHelpers;

namespace SplitProcessor
{
    public class SplitTransaction : Transaction
    {
        public const string SplitCategoryString = "Split/Multiple Categories";
        private CSVEntry headerEntry;
        private FileHelperEngine<CSVEntry> engine;
        private List<CSVEntry> SplitSubEntries;
        private bool TransComplete;

        public CSVEntry HeaderEntry
        {
            get
            {
                return this.headerEntry;
            }

            set
            {
                if (this.headerEntry != null)
                    throw new ApplicationException("Trying to set another split header to an existing split transaction that already has a header set.");

                this.headerEntry = value;
            }
        }

        public override bool IsTransactionComplete
        {
            get
            {
                if (this.SplitSubEntries == null || this.HeaderEntry == null)
                    return false;

                decimal subTotal = 0M;
                foreach (var subEntry in this.SplitSubEntries)
                {
                    subTotal += subEntry.Amount;
                }
                if (subTotal == this.HeaderEntry.Amount)
                    return true;

                return false;
            }
        }

        public SplitTransaction()
        {
            this.SplitSubEntries = new List<CSVEntry>();
            this.TransComplete = false;
            this.engine = new FileHelperEngine<CSVEntry>();
        }

        public override bool AddEntry(CSVEntry subentry)
        {
            if (this.IsTransactionComplete)
                throw new ApplicationException("Shouldn't be adding another entry to the split transaction when it's already complete.");

            if (IsSplitHeader(subentry) && this.HeaderEntry == null)
            {
                this.HeaderEntry = subentry;
            }
            else if (IsSplitSubEntry(subentry))
            {
                this.SplitSubEntries.Add(subentry);
            }
            else
            {
                throw new ApplicationException("Trying to add a standard transaction entry (or another split header) to an existing unfinished split transaction.");
            }

            return true;
        }

        public override string FullTransactionString()
        {
            var builder = new StringBuilder();
            builder.AppendLine(this.engine.WriteString(new[] { this.HeaderEntry }).TrimEnd('\r', '\n'));

            // parse the header string to get: Num (0), Date(1), Payee(2) and Account(3)
            foreach (var entry in this.SplitSubEntries)
            {
                entry.Number = this.HeaderEntry.Number;
                entry.TransactionDate = this.HeaderEntry.TransactionDate;
                entry.Payee = this.HeaderEntry.Payee;
                entry.Account = this.HeaderEntry.Account;
                builder.AppendLine(this.engine.WriteString(new[] { entry }).TrimEnd('\r', '\n'));
            }

            return builder.ToString();
        }

        /// <summary>
        /// Indiciates if the <see cref="CSVEntry"/> indicates that it's a split header.
        /// </summary>
        /// <param name="entry"><see cref="CSVEntry"/> to check.</param>
        /// <returns><c>true</c> if a split header. <c>false</c> otherwise.</returns>
        public static bool IsSplitHeader(CSVEntry entry)
        {
            if (entry.CategoryString.Contains(SplitCategoryString))
                return true;
            return false;
        }

        /// <summary>
        /// Uses the presence or absence of a valid date in the line to determine whether
        /// this is part of a multi-line split transaction.
        /// </summary>
        /// <remarks>Note that this does not detect the Split header.</remarks>
        /// <param name="entry">line to examine</param>
        /// <returns><c>true</c> if believed to a sub-entry of a split transaction. <c>false</c> otherwise.</returns>
        public static bool IsSplitSubEntry(CSVEntry entry)
        {
            if (entry == null)
                return false;

            return !entry.TransactionDate.HasValue;
        }
    }
}
