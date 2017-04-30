using Microsoft.VisualStudio.TestTools.UnitTesting;
using SplitProcessor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SplitProcessor.Tests
{
    [TestClass()]
    public class TransactionFactoryTests
    {
        public static class SplitTestData
        {
            public static CSVEntry[] GetSplit1()
            {
                var entry1 = new CSVEntry
                {
                    Number = 123,
                    Account = "Account 1",
                    Amount = 100.00M,
                    CategoryString = SplitTransaction.SplitCategoryString,
                    Classification = "Big Purchase",
                    Memo = "A really awesome thing.",
                    Payee = "Evil corp",
                    TransactionDate = new DateTime(2001, 12, 25)
                };

                var entry2 = new CSVEntry
                {
                    Amount = 20.00M,
                    CategoryString = "Groceries",
                    Memo = "Part 1.",
                };
                var entry3 = new CSVEntry
                {
                    Amount = 80.00M,
                    CategoryString = "Gifts",
                    Memo = "Part 2.",
                };

                return new[] { entry1, entry2, entry3 };
            }
        }
        [TestMethod()]
        public void BasicStandardTransactionTest()
        {
            // setup
            var singleCSVEntry = new CSVEntry
            {
                Number = 123,
                Account = "Account 1",
                Amount = 100.00M,
                CategoryString = "Category 1",
                Classification = "Big Purchase",
                Memo = "A really awesome thing.",
                Payee = "Evil corp",
                TransactionDate = new DateTime(2001, 12, 25)
            };

            // test
            var factory = new TransactionFactory(new[] { singleCSVEntry });
            var result = factory.Single();

            // result
            var expected = "123,2001-12-25,\"Evil corp\",Account 1,\"A really awesome thing.\",\"Category 1\",Big Purchase,100.00\r\n";
            Assert.AreEqual(expected,result);
        }

        [TestMethod()]
        public void BasicSplitTransactionTest()
        {
            // setup
            var split1 = SplitTestData.GetSplit1();            

            // test
            var factory = new TransactionFactory(split1);
            var result = factory.Single();

            // result

            //ToDo: This won't work at the moment because there's no standard transaction to follow through and flush the buffer.
            // This isn't great design. Better to 1) Detect the end of a split by summing the parts against the header. 2) return
            // transactions not transaction strings. this should allow easier testing.

            var expected = "123,2001-12-25,\"Evil corp\",Account 1,\"A really awesome thing.\",\"Split/Multiple Categories\",Big Purchase,100.00\r\n" +
                           "123,2001-12-25,\"Evil corp\",Account 1,\"Part 1.\",\"Groceries\",,20.00\r\n" +
                           "123,2001-12-25,\"Evil corp\",Account 1,\"Part 2.\",\"Gifts\",,80.00\r\n";
            Assert.AreEqual(expected, result);
        }


        [TestMethod()]
        public void TwoSplits()
        {
            // setup
            var entry1 = new CSVEntry
            {
                Number = 123,
                Account = "Account 1",
                Amount = 100.00M,
                CategoryString = SplitTransaction.SplitCategoryString,
                Classification = "Big Purchase",
                Memo = "A really awesome thing.",
                Payee = "Evil corp",
                TransactionDate = new DateTime(2001, 12, 25)
            };

            var entry2 = new CSVEntry
            {
                Amount = 20.00M,
                CategoryString = "Groceries",
                Memo = "Part 1.",
            };
            var entry3 = new CSVEntry
            {
                Amount = 80.00M,
                CategoryString = "Gifts",
                Memo = "Part 2.",
            };

            var entry4 = new CSVEntry
            {
                Number = 123,
                Account = "Account 1",
                Amount = 101.00M,
                CategoryString = SplitTransaction.SplitCategoryString,
                Classification = "Big Purchase",
                Memo = "A really awesome thing.",
                Payee = "Evil corp",
                TransactionDate = new DateTime(2001, 12, 25)
            };

            var entry5 = new CSVEntry
            {
                Amount = 20.00M,
                CategoryString = "Groceries",
                Memo = "Part 1.",
            };
            var entry6 = new CSVEntry
            {
                Amount = 81.00M,
                CategoryString = "Gifts",
                Memo = "Part 2.",
            };

            // test
            var factory = new TransactionFactory(new[] { entry1, entry2, entry3, entry4, entry5, entry6 });
            var result = factory.Aggregate((x,y)=> { return x + y; }) ;

            // result

            //ToDo: This won't work at the moment because there's no standard transaction to follow through and flush the buffer.
            // This isn't great design. Better to 1) Detect the end of a split by summing the parts against the header. 2) return
            // transactions not transaction strings. this should allow easier testing.

            var expected = "123,2001-12-25,\"Evil corp\",Account 1,\"A really awesome thing.\",\"Split/Multiple Categories\",Big Purchase,100.00\r\n" +
                           "123,2001-12-25,\"Evil corp\",Account 1,\"Part 1.\",\"Groceries\",,20.00\r\n" +
                           "123,2001-12-25,\"Evil corp\",Account 1,\"Part 2.\",\"Gifts\",,80.00\r\n" +
                           "123,2001-12-25,\"Evil corp\",Account 1,\"A really awesome thing.\",\"Split/Multiple Categories\",Big Purchase,101.00\r\n" +
                           "123,2001-12-25,\"Evil corp\",Account 1,\"Part 1.\",\"Groceries\",,20.00\r\n" +
                           "123,2001-12-25,\"Evil corp\",Account 1,\"Part 2.\",\"Gifts\",,81.00\r\n";
            Assert.AreEqual(expected, result);
        }
    }
}