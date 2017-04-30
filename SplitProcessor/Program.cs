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
                var entries = GetEntriesFromLines(lines);
                var transactionFactory = new TransactionFactory(entries);
                foreach (var singleTransaction in transactionFactory)
                {
                    writer.Write(singleTransaction);
                }
            }
        }

        /// <summary>
        /// Returns an array of all the csv lines from the file.
        /// ToDo: Make this lazy in line with the rest of the application. Also, refactor into separate class.
        /// </summary>
        /// <param name="lines">Non-blank lines to parse.</param>
        /// <returns>Array of <see cref="CSVEntry"/> objects representing the data contained in the file.</returns>
        private static CSVEntry[] GetEntriesFromLines(NonBlankFileLines lines)
        {
            var engine = new FileHelperEngine<CSVEntry>();
            var aggregateLines = lines.Aggregate((working, next) => { return working + Environment.NewLine + next; });
            aggregateLines = aggregateLines.TrimEnd('\r', '\n', '\0');
            var entries = engine.ReadString(aggregateLines);
            return entries;
        }
    }
}
