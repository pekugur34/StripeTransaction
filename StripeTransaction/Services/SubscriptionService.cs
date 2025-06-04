using System.Collections.Generic;
using System.Threading.Tasks;
using Stripe;

namespace StripeTransaction.Services
{
    public class SubscriptionService
    {
        private readonly Stripe.SubscriptionService _stripeService;

        public SubscriptionService()
        {
            _stripeService = new Stripe.SubscriptionService();
        }

        public async Task<Subscription> CreateAsync(string customerId, string priceId, int? trialPeriodDays = null)
        {
            return await _stripeService.CreateAsync(new SubscriptionCreateOptions
            {
                Customer = customerId,
                Items = new List<SubscriptionItemOptions>
                {
                    new SubscriptionItemOptions
                    {
                        Price = priceId,
                        Quantity = 1
                    }
                },
                TrialPeriodDays = trialPeriodDays,
                PaymentSettings = new SubscriptionPaymentSettingsOptions
                {
                    PaymentMethodTypes = new List<string> { "card" },
                    SaveDefaultPaymentMethod = "on_subscription"
                }
            });
        }

        public async Task CancelAsync(string subscriptionId)
        {
            await _stripeService.CancelAsync(subscriptionId);
        }
    }
} 