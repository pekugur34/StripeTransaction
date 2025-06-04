using System.Threading.Tasks;
using Stripe;

namespace StripeTransaction
{
    public interface IStripeOperation
    {
        /// <summary>
        /// Executes the Stripe operation
        /// </summary>
        Task ExecuteAsync();

        /// <summary>
        /// Rolls back the operation if it fails
        /// </summary>
        Task RollbackAsync();
    }

    public interface IStripeOperation<T> where T : class
    {
        /// <summary>
        /// The result of the operation
        /// </summary>
        T Result { get; }

        Task<T> ExecuteAsync();
        Task RollbackAsync();
    }
} 