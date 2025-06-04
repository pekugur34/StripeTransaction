using System.Threading.Tasks;
using Stripe;

namespace StripeTransaction.Services
{
    public class CustomerService
    {
        private readonly Stripe.CustomerService _stripeService;

        public CustomerService()
        {
            _stripeService = new Stripe.CustomerService();
        }

        public async Task<Customer> CreateAsync(string email, string name)
        {
            return await _stripeService.CreateAsync(new CustomerCreateOptions
            {
                Email = email,
                Name = name
            });
        }

        public async Task DeleteAsync(string customerId)
        {
            await _stripeService.DeleteAsync(customerId);
        }

        public async Task<Customer> UpdateAsync(string customerId, string description)
        {
            return await _stripeService.UpdateAsync(customerId, new CustomerUpdateOptions
            {
                Description = description
            });
        }
    }
} 