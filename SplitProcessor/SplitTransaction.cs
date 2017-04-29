using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using FileHelpers;

namespace SplitProcessor
{
    public class SplitTransaction : Transaction
    {
        private CSVEntry headerEntry;
        private FileHelperEngine<CSVEntry> engine = new FileHelperEngine<CSVEntry>();
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

        private List<CSVEntry> SplitSubEntries;
        private bool TransComplete;

        public SplitTransaction()
        {
            this.SplitSubEntries = new List<CSVEntry>();
            TransComplete = false;
        }

        public override bool AddEntry(CSVEntry subentry)
        {
            //this.TransComplete = false;
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
            return this.TransComplete;
        }

        public override string FullTransactionString()
        {
            var builder = new StringBuilder();
            builder.AppendLine(this.engine.WriteString(new[] { this.HeaderEntry }).TrimEnd('\r', '\n'));
            //builder.AppendLine(this.HeaderEntry.ToDelimitedString());

            // ToDo: get the coloumn indexes from the file header.
            // parse the header string to get: Num (0), Date(1), Payee(2) and Account(3)
            foreach (var entry in this.SplitSubEntries)
            {
                entry.Number = this.HeaderEntry.Number;
                entry.TransactionDate = this.HeaderEntry.TransactionDate;
                entry.Payee = this.HeaderEntry.Payee;
                entry.Account = this.HeaderEntry.Account;
                builder.AppendLine(this.engine.WriteString(new[] { entry }).TrimEnd('\r', '\n'));

                //var parts = line.Split(',');
                //ReplaceSegments(0, parts);
                //ReplaceSegments(1, parts);
                //ReplaceSegments(2, parts);
                //ReplaceSegments(3, parts);
                //var result = parts.Aggregate((x, y) => { return x + "," + y; });
                //builder.AppendLine(result);
            }

            
            //engi
            return builder.ToString();
        }

        //private void ReplaceSegments(int index, string[] parts)
        //{
        //    if (string.IsNullOrWhiteSpace(parts[index]))
        //        parts[index] = this.HeaderEntry.Split(',')[index];
        //}

        public static bool IsSplitHeader(CSVEntry entry)
        {
            if (entry.CategoryString.Contains("Split/Multiple Categories"))
                return true;
            return false;
        }

        /// <summary>
        /// Uses the presence or absence of a valid date in the line to determine whether
        /// this is part of a multi-line split transaction.
        /// </summary>
        /// <remarks>Note that this does not detect the Split header.</remarks>
        /// <param name="entry">line to examins</param>
        /// <returns><c>true</c> if part of a split transaction.</returns>
        public static bool IsSplitSubEntry(CSVEntry entry)
        {
            if (entry == null)
                return false;

            //var stringSections = entry.Split(',');
            //if (stringSections.Count() == 1)
            //    return false;

            return !entry.TransactionDate.HasValue;

            //bool dateFound = false;
            //foreach (var substring in stringSections)
            //{
            //    DateTime date;
            //    dateFound |= DateTime.TryParse(substring, out date);
            //}
            //return !dateFound;
        }
    }
}
