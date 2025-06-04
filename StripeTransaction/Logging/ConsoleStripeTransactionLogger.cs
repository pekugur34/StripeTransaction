using System;

namespace StripeTransaction.Logging
{
    public class ConsoleStripeTransactionLogger : IStripeTransactionLogger
    {
        public void LogInformation(string message)
        {
            Console.WriteLine($"[INFO] {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} - {message}");
        }

        public void LogWarning(string message)
        {
            Console.WriteLine($"[WARN] {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} - {message}");
        }

        public void LogError(string message, Exception? exception = null)
        {
            Console.WriteLine($"[ERROR] {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} - {message}");
            if (exception != null)
            {
                Console.WriteLine($"Exception: {exception.Message}");
                Console.WriteLine($"Stack Trace: {exception.StackTrace}");
            }
        }

        public void LogDebug(string message)
        {
            Console.WriteLine($"[DEBUG] {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} - {message}");
        }
    }
} 