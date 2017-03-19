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
            string inputPath;
            if (args.Length != 1)
                return;
            else
                inputPath = args[0];

            var inputLines = new NonBlankFileLines(inputPath);

            var inputDir = Path.GetDirectoryName(inputPath);
            var inputFileName = Path.GetFileNameWithoutExtension(inputPath);
            var inputFileNameExtension = Path.GetExtension(inputPath);
            var outputPath = Path.Combine(inputDir, inputFileName + "_Processed" + inputFileNameExtension);

            using (var writer = new StreamWriter(outputPath))
            {
                writer.Write(inputLines.GetHeader);
                var trans = new TransactionFactory(inputLines);
                foreach(var line in trans)
                {
                    writer.Write(line);
                }
            }

        }


    }
}
