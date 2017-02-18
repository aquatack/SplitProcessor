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
                foreach(var line in inputLines)
                {
                    writer.WriteLine(line);
                }
            }

        }


    }
}
