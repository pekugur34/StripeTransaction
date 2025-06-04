using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Stripe;
using StripeTransaction.Logging;

namespace StripeTransaction
{
    public class StripeTransactionManager : IDisposable
    {
        private readonly List<Func<Task>> _rollbacks;
        private readonly IStripeTransactionLogger _logger;
        private bool _isCommitted;
        private bool _isDisposed;

        public StripeTransactionManager(IStripeTransactionLogger? logger = null)
        {
            if (string.IsNullOrEmpty(StripeConfiguration.ApiKey))
                throw new InvalidOperationException("Stripe API key is not initialized. Call StripeTransactionConfiguration.Initialize() first.");

            _rollbacks = new List<Func<Task>>();
            _logger = logger ?? new ConsoleStripeTransactionLogger();
            _isCommitted = false;
            _isDisposed = false;

            _logger.LogDebug("StripeTransactionManager initialized");
        }

        public async Task<T?> ExecuteAsync<T>(Func<Task<T>> operation) where T : class
        {
            if (_isCommitted || _isDisposed)
            {
                var error = "Cannot add operations to a committed or disposed transaction.";
                _logger.LogError(error);
                throw new InvalidOperationException(error);
            }

            try
            {
                _logger.LogDebug($"Executing operation that returns {typeof(T).Name}");
                var result = await operation();
                
                if (result != null)
                {
                    _logger.LogDebug($"Operation successful, adding rollback for {typeof(T).Name}");
                    _rollbacks.Add(() => GetRollbackOperation(result));
                }
                else
                {
                    _logger.LogWarning($"Operation returned null for type {typeof(T).Name}");
                }
                
                return result;
            }
            catch (StripeException ex)
            {
                _logger.LogError($"Stripe API error: {ex.Message}", ex);
                await RollbackAsync();
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Unexpected error during operation execution: {ex.Message}", ex);
                await RollbackAsync();
                throw;
            }
        }

        public async Task ExecuteAsync(Func<Task> operation)
        {
            if (_isCommitted || _isDisposed)
            {
                var error = "Cannot add operations to a committed or disposed transaction.";
                _logger.LogError(error);
                throw new InvalidOperationException(error);
            }

            try
            {
                _logger.LogDebug("Executing void operation");
                await operation();
                _logger.LogDebug("Void operation completed successfully");
            }
            catch (StripeException ex)
            {
                _logger.LogError($"Stripe API error: {ex.Message}", ex);
                await RollbackAsync();
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Unexpected error during operation execution: {ex.Message}", ex);
                await RollbackAsync();
                throw;
            }
        }

        private Task GetRollbackOperation<T>(T result) where T : class
        {
            if (result == null)
            {
                _logger.LogWarning($"Cannot create rollback operation for null result of type {typeof(T).Name}");
                return Task.CompletedTask;
            }

            _logger.LogDebug($"Determining rollback operation for {typeof(T).Name}");
            
            return result switch
            {
                Customer customer => RollbackCustomer(customer),
                PaymentMethod paymentMethod => RollbackPaymentMethod(paymentMethod),
                Subscription subscription => RollbackSubscription(subscription),
                WebhookEndpoint webhookEndpoint => RollbackWebhookEndpoint(webhookEndpoint),
                PaymentIntent paymentIntent => RollbackPaymentIntent(paymentIntent),
                Invoice invoice => RollbackInvoice(invoice),
                _ => Task.CompletedTask
            };
        }

        private async Task RollbackCustomer(Customer customer)
        {
            try
            {
                _logger.LogInformation($"Rolling back customer creation: {customer.Id}");
                await new CustomerService().DeleteAsync(customer.Id);
                _logger.LogInformation($"Successfully deleted customer: {customer.Id}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to rollback customer creation: {customer.Id}", ex);
                throw;
            }
        }

        private async Task RollbackPaymentMethod(PaymentMethod paymentMethod)
        {
            try
            {
                _logger.LogInformation($"Rolling back payment method: {paymentMethod.Id}");
                await new PaymentMethodService().DetachAsync(paymentMethod.Id);
                _logger.LogInformation($"Successfully detached payment method: {paymentMethod.Id}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to rollback payment method: {paymentMethod.Id}", ex);
                throw;
            }
        }

        private async Task RollbackSubscription(Subscription subscription)
        {
            try
            {
                _logger.LogInformation($"Rolling back subscription: {subscription.Id}");
                await new SubscriptionService().CancelAsync(subscription.Id);
                _logger.LogInformation($"Successfully cancelled subscription: {subscription.Id}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to rollback subscription: {subscription.Id}", ex);
                throw;
            }
        }

        private async Task RollbackWebhookEndpoint(WebhookEndpoint webhookEndpoint)
        {
            try
            {
                _logger.LogInformation($"Rolling back webhook endpoint: {webhookEndpoint.Id}");
                await new WebhookEndpointService().DeleteAsync(webhookEndpoint.Id);
                _logger.LogInformation($"Successfully deleted webhook endpoint: {webhookEndpoint.Id}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to rollback webhook endpoint: {webhookEndpoint.Id}", ex);
                throw;
            }
        }

        private async Task RollbackPaymentIntent(PaymentIntent paymentIntent)
        {
            try
            {
                _logger.LogInformation($"Rolling back payment intent: {paymentIntent.Id}");
                await new PaymentIntentService().CancelAsync(paymentIntent.Id);
                _logger.LogInformation($"Successfully cancelled payment intent: {paymentIntent.Id}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to rollback payment intent: {paymentIntent.Id}", ex);
                throw;
            }
        }

        private async Task RollbackInvoice(Invoice invoice)
        {
            try
            {
                _logger.LogInformation($"Rolling back invoice: {invoice.Id}");
                await new InvoiceService().VoidInvoiceAsync(invoice.Id);
                _logger.LogInformation($"Successfully voided invoice: {invoice.Id}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to rollback invoice: {invoice.Id}", ex);
                throw;
            }
        }

        private async Task RollbackAsync()
        {
            if (_isDisposed)
            {
                var error = "Cannot rollback a disposed transaction.";
                _logger.LogError(error);
                throw new InvalidOperationException(error);
            }

            _logger.LogInformation($"Starting rollback of {_rollbacks.Count} operations");
            
            for (int i = _rollbacks.Count - 1; i >= 0; i--)
            {
                try
                {
                    await _rollbacks[i]();
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Rollback operation {i + 1} failed", ex);
                    // Continue with other rollbacks
                }
            }
            
            _logger.LogInformation("Rollback completed");
        }

        public void Dispose()
        {
            if (!_isDisposed)
            {
                if (!_isCommitted)
                {
                    _logger.LogWarning("Transaction was not committed, performing rollback during disposal");
                    RollbackAsync().GetAwaiter().GetResult();
                }
                _isDisposed = true;
                _logger.LogDebug("StripeTransactionManager disposed");
            }
        }
    }
} 