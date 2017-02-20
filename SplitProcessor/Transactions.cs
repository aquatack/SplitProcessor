using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SplitProcessor
{
    public class Transactions : IEnumerable<string>
    {
        private IEnumerable<string> fileLines;
        private IEnumerator fileLinesEnumerator;
        public Transactions(IEnumerable<string> fileLines)
        {
            this.fileLines = fileLines;
            this.fileLinesEnumerator = fileLines.GetEnumerator();
        }

        public IEnumerator<string> GetEnumerator()
        {
            bool inSplit = false;
            var builder = new StringBuilder();
            foreach (var line in this.fileLines)
            {
                if(inSplit)
                {
                    if (IsSplitSubEntry(line))
                        builder.AppendLine("!!!!"+line);
                    else
                    {
                        inSplit = false;
                        // also need to add the next line
                        builder.AppendLine(line);
                        yield return builder.ToString();
                        
                    }
                    continue;
                }

                if (!IsSplitHeader(line))
                {
                    yield return line + Environment.NewLine;
                }
                else
                {
                    // just detected a split header.
                    builder = new StringBuilder();
                    builder.AppendLine(line);
                    inSplit = true;
                }
                
            }
        }

        /// <summary>
        /// Uses the presence or absence of a valid date in the line to determine whether
        /// this is part of a multi-line split transaction.
        /// </summary>
        /// <remarks>Note that this does not detect the Split header.</remarks>
        /// <param name="line">line to examins</param>
        /// <returns><c>true</c> if part of a split transaction.</returns>
        private bool IsSplitSubEntry(string line)
        {
            if (string.IsNullOrWhiteSpace(line))
                return true;

            var stringSections = line.Split(',');
            bool dateFound = false;
            foreach (var substring in stringSections)
            {
                DateTime date;
                dateFound |= DateTime.TryParse(substring, out date);
            }
            return !dateFound;
        }

        private bool IsSplitHeader(string line)
        {
            if (line.Contains(",Split/Multiple Categories,"))
                return true;
            return false;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
