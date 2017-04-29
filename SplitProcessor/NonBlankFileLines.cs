using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FileHelpers;

namespace SplitProcessor
{
    public class NonBlankFileLines : IEnumerable<string>, IDisposable
    {
        private StreamReader reader;
        private string header;
        private IEnumerable<CSVEntry> body;

        public string GetHeader { get { return this.header; } }

        public NonBlankFileLines(string filename = @"C:\temp\123.csv")
        {
            this.reader = new StreamReader(filename);
            this.header = RetrieveHeader();
            //RetrieveBody(this.reader);
        }

        private void RetrieveBody(StreamReader reader)
        {
            var engine = new FileHelperEngine<CSVEntry>();
            engine.ErrorManager.ErrorMode = ErrorMode.IgnoreAndContinue;
            this.body = engine.ReadStream(reader);
        }

        private string RetrieveHeader()
        {
            var header = new StringBuilder();
            while (!this.reader.EndOfStream)
            {
                var line = this.reader.ReadLine();
                header.AppendLine(line);
                if (line.Contains("Date") && line.Contains("Payee") && line.Contains("Amount"))
                {
                    return header.ToString();
                }
            }
            throw new ApplicationException("Can't find a header line");
        }

        private bool IsFooter(string line)
        {
            if (line.StartsWith("Grand Total"))
                return true;
            return false;
        }

        public void Dispose()
        {
            this.reader.Close();
        }        

        public IEnumerator<string> GetEnumerator()
        {
            //return this.body.GetEnumerator();
            while (!this.reader.EndOfStream)
            {
                var line = this.reader.ReadLine();
                if (!string.IsNullOrWhiteSpace(line) && !IsFooter(line))
                    yield return line;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
