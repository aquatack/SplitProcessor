using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SplitProcessor
{
    public class SplitTransaction
    {
        public string HeaderString { get; set; }
        private List<string> SplitSubLines;

        public SplitTransaction()
        {
            this.SplitSubLines = new List<string>();
        }

        public void AddSplitSubLine(string subline)
        {
            this.SplitSubLines.Add(subline);
        }

        public string GetSplitTransactionString()
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
    }
}
