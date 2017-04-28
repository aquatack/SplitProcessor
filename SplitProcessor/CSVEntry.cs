using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FileHelpers;

namespace SplitProcessor
{
    [DelimitedRecord(",")]
    public class CSVEntry
    {
        private const char DELIMITER = ',';

        public int? Number;

        [FieldConverter(ConverterKind.Date, "yyyy-MM-dd")]
        public DateTime? TransactionDate;

        public string Payee;

        public string Account;

        public string Memo;

        public string CategoryString;

        public string Classification;

        [FieldConverter(ConverterKind.Decimal, ".")]
        public decimal Amount;

        public string ToDelimitedString()
        {
            var builder = new StringBuilder();
            builder.Append((this.Number.HasValue ? this.Number.Value.ToString() : "") + DELIMITER);
            builder.Append((this.TransactionDate.HasValue ? this.TransactionDate.Value.ToString("yyyy-MM-dd") : "") + DELIMITER);
            builder.Append((!string.IsNullOrEmpty(this.Payee) ? this.Payee.ToString() : "") + DELIMITER);
            builder.Append((!string.IsNullOrEmpty(this.Account) ? this.Account.ToString() : "") + DELIMITER);
            builder.Append((!string.IsNullOrEmpty(this.Memo) ? this.Memo.ToString() : "") + DELIMITER);
            builder.Append((!string.IsNullOrEmpty(this.CategoryString) ? this.CategoryString.ToString() : "") + DELIMITER);
            builder.Append((!string.IsNullOrEmpty(this.Classification) ? this.Classification.ToString() : "") + DELIMITER);
            builder.Append(this.Amount);

            return builder.ToString();
        }
    }
}
