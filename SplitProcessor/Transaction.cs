namespace SplitProcessor
{
    /// <summary>
    /// Represents an atomic transaction.
    /// </summary>
    public abstract class Transaction
    {
        /// <summary>
        /// Method to a new line entry into the transaction.
        /// </summary>
        /// <remarks>For split transactions, this might be called
        /// multiple times to build up the complete representation of the transaction.</remarks>
        /// <param name="entry">A CSVEntry containing some or all of the transaction data.</param>
        /// <returns>true if the entry was valid for the transaction and successfully added.</returns>
        public abstract bool AddEntry(CSVEntry entry);

        /// <summary>
        /// Indicates whether sufficient information has been provided for the transaction
        /// to represent a complete entity.
        /// </summary>
        /// <returns><c>true</c> if the transaction is complete.</returns>
        public abstract bool TransactionComplete();

        /// <summary>
        /// The delimited string representation of the transaction.
        /// </summary>
        /// <returns>Returns the text representation of the transaction.</returns>
        public abstract string FullTransactionString();
    }
}
