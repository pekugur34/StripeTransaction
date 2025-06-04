using System.Threading.Tasks;
using Stripe;

namespace StripeTransaction.Services
{
    public class PaymentMethodService
    {
        private readonly Stripe.PaymentMethodService _stripeService;

        public PaymentMethodService()
        {
            _stripeService = new Stripe.PaymentMethodService();
        }

        public async Task<PaymentMethod> CreateAsync(string cardNumber, int expMonth, int expYear, string cvc)
        {
            return await _stripeService.CreateAsync(new PaymentMethodCreateOptions
            {
                Type = "card",
                Card = new PaymentMethodCardOptions
                {
                    Number = cardNumber,
                    ExpMonth = expMonth,
                    ExpYear = expYear,
                    Cvc = cvc
                }
            });
        }

        public async Task AttachAsync(string paymentMethodId, string customerId)
        {
            await _stripeService.AttachAsync(paymentMethodId, new PaymentMethodAttachOptions
            {
                Customer = customerId
            });
        }

        public async Task DetachAsync(string paymentMethodId)
        {
            await _stripeService.DetachAsync(paymentMethodId);
        }
    }
} 