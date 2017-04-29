using System;
using System.IO;
using System.Linq;

using FileHelpers;

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

            var inputDir = Path.GetDirectoryName(inputPath);
            var inputFileName = Path.GetFileNameWithoutExtension(inputPath);
            var inputFileNameExtension = Path.GetExtension(inputPath);
            var outputPath = Path.Combine(inputDir, inputFileName + "_Processed" + inputFileNameExtension);

            var lines = new NonBlankFileLines(inputPath);

            using (var writer = new StreamWriter(outputPath))
            {
                writer.Write(lines.GetHeader);

                var engine = new FileHelperEngine<CSVEntry>();
                var aggregateLines = lines.Aggregate((working, next) => { return working + Environment.NewLine + next; });
                aggregateLines = aggregateLines.TrimEnd('\r','\n','\0');
                var entries = engine.ReadString(aggregateLines);

                var trans = new TransactionFactory(entries);
                foreach (var line in trans)
                {
                    writer.Write(line);
                }
            }
        }
    }
}
