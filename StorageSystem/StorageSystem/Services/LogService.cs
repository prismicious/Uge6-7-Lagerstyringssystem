using System;
using System.IO;
using StorageSystem.Models;

namespace StorageSystem.Services
{
    public static class LogService
    {
        // Path to the CSV file for transaction logs => root/StorageSystem/logs/transactions.csv
        private static readonly string TransactionLogsCsvFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs", "transactions.csv"); 

        // Log a message
        public static void Log(string message)
        {
            Console.WriteLine($"[{DateTime.Now}] {message}");
        }

        // Log an error
        public static void LogError(string error)
        {
            Console.WriteLine($"[{DateTime.Now}] ERROR: {error}");
        }

        // Generic function to log a string to a CSV file
        public static void LogStringToCsv(string csvLine, string filePath, string header = null)
        {
            try
            {
                string directoryPath = Path.GetDirectoryName(filePath) ?? string.Empty;
                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }

                bool fileExists = File.Exists(filePath);

                using (var writer = new StreamWriter(filePath, append: true))
                {
                    // Write header if the file is being created
                    if (!fileExists && !string.IsNullOrEmpty(header))
                    {
                        writer.WriteLine(header);
                    }

                    // Write the CSV line
                    writer.WriteLine(csvLine);
                }
            }
            catch (Exception ex)
            {
                LogError($"Failed to log to CSV: {ex.Message}");
            }
        }

        // Refactor LogTransactionToCsv to use LogStringToCsv
        public static void LogTransaction(Transaction transaction, string? path)
        {
            if (string.IsNullOrEmpty(path))
            {
                path = TransactionLogsCsvFilePath;
            }

            if (transaction == null)
            {
                LogError("Transaction is null. Cannot save to CSV.");
                return;
            }

            string header = "ID,Date,Type,OrderListID,Receipt,Warehouse";
            LogStringToCsv(transaction.ToString(), path, header);
            Log($"Transaction ID {transaction.ID} saved to CSV.");
        }
    }
}
