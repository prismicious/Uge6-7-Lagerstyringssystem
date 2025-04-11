using Microsoft.VisualStudio.TestTools.UnitTesting;
using StorageSystem.Models;
using StorageSystem.Services;
using System;
using System.IO;

namespace StorageSystemTest
{
    [TestClass]
    public class TestLogService
    {
        private readonly string testCsvFilePath = "logs/transactions.csv";

        [TestInitialize]
        public void Setup()
        {
            // Ensure the test CSV file is clean before each test
            if (File.Exists(testCsvFilePath))
            {
                File.Delete(testCsvFilePath);
            }
        }

        [TestCleanup]
        public void Cleanup()
        {
            // Clean up the test CSV file after each test
            if (File.Exists(testCsvFilePath))
            {
                File.Delete(testCsvFilePath);
            }
        }

        [TestMethod]
        public void TestLogStringToCsv_CreatesFileWithHeader()
        {
            string header = "ID,Date,Type";
            string csvLine = "1,2023-01-01,Test";

            LogService.LogStringToCsv(csvLine, testCsvFilePath, header);

            Assert.IsTrue(File.Exists(testCsvFilePath));
            var lines = File.ReadAllLines(testCsvFilePath);
            Assert.AreEqual(2, lines.Length);
            Assert.AreEqual(header, lines[0]);
            Assert.AreEqual(csvLine, lines[1]);
        }

        [TestMethod]
        public void TestLogStringToCsv_AppendsToFile()
        {
            string header = "ID,Date,Type";
            string csvLine1 = "1,2023-01-01,Test1";
            string csvLine2 = "2,2023-01-02,Test2";

            LogService.LogStringToCsv(csvLine1, testCsvFilePath, header);
            LogService.LogStringToCsv(csvLine2, testCsvFilePath);

            var lines = File.ReadAllLines(testCsvFilePath);
            Assert.AreEqual(3, lines.Length);
            Assert.AreEqual(csvLine1, lines[1]);
            Assert.AreEqual(csvLine2, lines[2]);
        }

        [TestMethod]
        public void TestLogTransaction_LogsTransaction()
        {
            // Arrange
            var transaction = new Transaction
            {
                ID = 1,
                Date = DateTime.Now,
                Type = 0,
                OrderListID = 123,
                Receipt = new Receipt
                {
                    ID = 1,
                    Date = DateTime.Now,
                    TransactionID = 1,
                    Transaction = null
                }
            }; 

            // Act
            LogService.LogTransaction(transaction, testCsvFilePath);

            // Assert
            Assert.IsTrue(File.Exists(testCsvFilePath));
            var lines = File.ReadAllLines(testCsvFilePath);
            Assert.IsTrue(lines.Length > 1); 
            Assert.IsTrue(lines[1].Contains(transaction.ID.ToString()));
            Assert.IsTrue(lines[1].Contains(transaction.Receipt.ID.ToString()));
        }

        [TestMethod]
        public void TestLogTransaction_NullTransaction()
        {
            using (var sw = new StringWriter())
            {
                Console.SetOut(sw);

                LogService.LogTransaction(null, null);

                var output = sw.ToString();
                Assert.IsTrue(output.Contains("Transaction is null. Cannot save to CSV."));
            }
        }
    }
}
