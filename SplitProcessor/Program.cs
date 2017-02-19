using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SplitProcessor
{
    class Program
    {
        static void Main(string[] args)
        {
            var inputLines = new NonBlankFileLines();

            using (var writer = new StreamWriter(@"C:\temp\output123.txt"))
            {

                writer.Write(inputLines.GetHeader);
                var trans = new Transactions(inputLines);
                foreach(var line in trans)
                {
                    writer.Write(line);
                }
            }

        }


    }
}
