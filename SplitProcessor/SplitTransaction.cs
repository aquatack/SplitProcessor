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
            // ToDo: get the coloumn indexes from the file header.
            // parse the header string to get: Num (0), Date(1), Payee(2) and Account(3)
            foreach(var line in this.SplitSubLines)
            {
                var parts = line.Split(',');
                if (string.IsNullOrWhiteSpace(parts[0]))
                    parts[0] = this.HeaderString.Split(',')[0];
            }
            return null;
        }
    }
}
