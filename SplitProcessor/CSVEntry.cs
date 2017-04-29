using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FileHelpers;

namespace SplitProcessor
{
    [DelimitedRecord(",")]
    [IgnoreEmptyLines]
    public class CSVEntry
    {
        private const char DELIMITER = ',';

        public int? Number;

        [FieldConverter(ConverterKind.Date, "yyyy-MM-dd")]
        public DateTime? TransactionDate;

        [FieldQuoted]
        public string Payee;

        public string Account;

        [FieldQuoted]
        public string Memo;

        [FieldQuoted]
        public string CategoryString;

        public string Classification;

        [FieldConverter(ConverterKind.Decimal, ".")]
        public decimal Amount;
    }
}
