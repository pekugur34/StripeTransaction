using System;

namespace StripeTransaction.Logging
{
    public interface IStripeTransactionLogger
    {
        void LogInformation(string message);
        void LogWarning(string message);
        void LogError(string message, Exception? exception = null);
        void LogDebug(string message);
    }
} 