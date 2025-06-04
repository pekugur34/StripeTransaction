using System;
using Stripe;

namespace StripeTransaction
{
    public static class StripeTransactionConfiguration
    {
        private static bool _isInitialized;

        public static void Initialize(string apiKey)
        {
            if (string.IsNullOrEmpty(apiKey))
                throw new ArgumentNullException(nameof(apiKey), "Stripe API key cannot be null or empty.");

            StripeConfiguration.ApiKey = apiKey;
            _isInitialized = true;
        }

        public static void Initialize(string apiKey, string apiVersion)
        {
            if (_isInitialized)
                return;

            StripeConfiguration.ApiKey = apiKey;
            _isInitialized = true;
        }
    }
} 