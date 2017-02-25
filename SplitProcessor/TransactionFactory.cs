using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SplitProcessor
{
    public class TransactionFactory : IEnumerable<string>
    {
        private IEnumerable<string> fileLines;
        //private IEnumerator fileLinesEnumerator;
        public TransactionFactory(IEnumerable<string> fileLines)
        {
            this.fileLines = fileLines;
            //this.fileLinesEnumerator = fileLines.GetEnumerator();
        }

        public IEnumerator<string> GetEnumerator()
        {
            Queue<string> tQueue = new Queue<string>();
            Transaction currentTransaction = null;
            foreach(var inputLine in this.fileLines)
            {
                if (currentTransaction == null)
                    currentTransaction = GetTranaction(inputLine);

                // If this fails, the split is complete. add to the queue.
                if (!currentTransaction.AddLine(inputLine))
                {
                    if (!currentTransaction.TransactionComplete())
                        throw new ApplicationException();

                    tQueue.Enqueue(currentTransaction.FullTransactionString());
                    currentTransaction = GetTranaction(inputLine);
                    if (!currentTransaction.AddLine(inputLine))
                        throw new ApplicationException();
                }
                while(tQueue.Any())
                {
                    yield return tQueue.Dequeue();
                }
                if (currentTransaction.TransactionComplete())
                {
                    yield return currentTransaction.FullTransactionString();
                    currentTransaction = null;
                }
            }
        }

        private Transaction GetTranaction(string inputLine)
        {
            if (StandardTransaction.IsStandardTransaction(inputLine))
                return new StandardTransaction();
            else if (SplitTransaction.IsSplitHeader(inputLine))
                return new SplitTransaction();
            else if (SplitTransaction.IsSplitSubEntry(inputLine))
                throw new ApplicationException();
            else
                throw new ApplicationException();
        }

        //public IEnumerator<string> GetEnumerator()
        //{
        //    bool inSplit = false;
        //    var builder = new StringBuilder();
        //    foreach (var line in this.fileLines)
        //    {
        //        if(inSplit)
        //        {
        //            if (IsSplitSubEntry(line))
        //                builder.AppendLine("!!!!"+line);
        //            else
        //            {
        //                inSplit = false;
        //                // also need to add the next line
        //                builder.AppendLine(line);
        //                yield return builder.ToString();

        //            }
        //            continue;
        //        }

        //        if (!IsSplitHeader(line))
        //        {
        //            yield return line + Environment.NewLine;
        //        }
        //        else
        //        {
        //            // just detected a split header.
        //            builder = new StringBuilder();
        //            builder.AppendLine(line);
        //            inSplit = true;
        //        }

        //    }
        //}



        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }


    }
}
