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
                    throw new ApplicationException();

                this.headerEntry = value;
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
                // if we're adding to a split transaction but it's not
                // either a header or split body, then this must be from the
                // next transaction. Indiciate that we're done and return
                // false.
                if (this.HeaderEntry != null)
                    this.TransComplete = true;
                else
                    this.TransComplete = false;

                return false;
            }

            return true;
        }

        public override bool TransactionComplete()
        {
            // ToDo: Check the sub entries all add up.
            return this.TransComplete;
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
        /// Indiciates if the CSVEntry indicates that it's a split header.
        /// </summary>
        /// <param name="entry">CSVEntry to check.</param>
        /// <returns><c>true</c> if a split header.</returns>
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
        /// <returns><c>true</c> if part of a split transaction.</returns>
        public static bool IsSplitSubEntry(CSVEntry entry)
        {
            if (entry == null)
                return false;

            return !entry.TransactionDate.HasValue;
        }
    }
}
