using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Stripe;

namespace StripeTransaction.Services
{
    public class WebhookEndpointService
    {
        private readonly Stripe.WebhookEndpointService _stripeService;

        public WebhookEndpointService()
        {
            _stripeService = new Stripe.WebhookEndpointService();
        }

        public async Task<WebhookEndpoint> CreateAsync(string url, IEnumerable<string> enabledEvents, string subscriptionId)
        {
            return await _stripeService.CreateAsync(new WebhookEndpointCreateOptions
            {
                Url = url,
                EnabledEvents = new List<string>(enabledEvents),
                Metadata = new Dictionary<string, string>
                {
                    { "subscriptionId", subscriptionId }
                }
            });
        }

        public async Task DeleteAsync(string webhookEndpointId)
        {
            await _stripeService.DeleteAsync(webhookEndpointId);
        }
    }
} 