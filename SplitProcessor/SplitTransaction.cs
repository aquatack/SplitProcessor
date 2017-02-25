using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SplitProcessor
{
    public class SplitTransaction : Transaction
    {
        private string headerString;
        public string HeaderString
        {
            get
            {
                return this.headerString;
            }

            set
            {
                if (!string.IsNullOrEmpty(this.headerString))
                    throw new ApplicationException();
                this.headerString = value;
            }
        }

        private List<string> SplitSubLines;
        private bool TransComplete;

        public SplitTransaction()
        {
            this.SplitSubLines = new List<string>();
            TransComplete = false;
        }

        public override bool AddLine(string subline)
        {
            //this.TransComplete = false;
            if (IsSplitHeader(subline) && string.IsNullOrEmpty(this.HeaderString))
            {
                this.HeaderString = subline;
            }
            else if (IsSplitSubEntry(subline))
            {
                this.SplitSubLines.Add(subline);
            }
            else
            {
                if (!string.IsNullOrEmpty(this.HeaderString))
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
            builder.AppendLine(this.HeaderString);
            // ToDo: get the coloumn indexes from the file header.
            // parse the header string to get: Num (0), Date(1), Payee(2) and Account(3)
            foreach(var line in this.SplitSubLines)
            {
                var parts = line.Split(',');
                ReplaceSegments(0, parts);
                ReplaceSegments(1, parts);
                ReplaceSegments(2, parts);
                ReplaceSegments(3, parts);
                var result = parts.Aggregate((x, y) => { return x + "," + y; });
                builder.AppendLine(result);
            }
            return builder.ToString();
        }

        private void ReplaceSegments(int index, string[] parts)
        {
            if (string.IsNullOrWhiteSpace(parts[index]))
                parts[index] = this.HeaderString.Split(',')[index];
        }

        public static bool IsSplitHeader(string line)
        {
            if (line.Contains(",Split/Multiple Categories,"))
                return true;
            return false;
        }

        /// <summary>
        /// Uses the presence or absence of a valid date in the line to determine whether
        /// this is part of a multi-line split transaction.
        /// </summary>
        /// <remarks>Note that this does not detect the Split header.</remarks>
        /// <param name="line">line to examins</param>
        /// <returns><c>true</c> if part of a split transaction.</returns>
        public static bool IsSplitSubEntry(string line)
        {
            if (string.IsNullOrWhiteSpace(line))
                return false;

            var stringSections = line.Split(',');
            if (stringSections.Count() == 1)
                return false;

            bool dateFound = false;
            foreach (var substring in stringSections)
            {
                DateTime date;
                dateFound |= DateTime.TryParse(substring, out date);
            }
            return !dateFound;
        }
    }
}
