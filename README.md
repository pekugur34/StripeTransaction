# Stripe Transaction

A .NET library that provides transaction-like behavior for Stripe operations, ensuring atomicity and rollback capabilities for complex Stripe operations.

## Features

- Transaction-like behavior for Stripe operations
- Automatic rollback on failure
- Comprehensive logging support
- Support for multiple Stripe operations:
  - Customer management
  - Payment method handling
  - Subscription management
  - Webhook endpoint management
  - Payment intent processing
  - Invoice management

## Installation

```bash
dotnet add package StripeTransaction
```

## Quick Start

1. Initialize the library with your Stripe API key:

```csharp
using StripeTransaction;

// Initialize with your Stripe API key
StripeTransactionConfiguration.Initialize("your_stripe_api_key");
```

2. Use the transaction in your code:

```csharp
using (var transaction = new StripeTransaction())
{
    // Create a customer
    var customer = await transaction.ExecuteAsync(async () =>
    {
        var customerService = new CustomerService();
        return await customerService.CreateAsync(new CustomerCreateOptions
        {
            Email = "customer@example.com",
            Name = "John Doe"
        });
    });

    // Add a payment method
    var paymentMethod = await transaction.ExecuteAsync(async () =>
    {
        var paymentMethodService = new PaymentMethodService();
        return await paymentMethodService.CreateAsync(new PaymentMethodCreateOptions
        {
            Type = "card",
            Card = new PaymentMethodCardOptions
            {
                Number = "4242424242424242",
                ExpMonth = 12,
                ExpYear = 2024,
                Cvc = "123"
            }
        });
    });

    // If any operation fails, all previous operations will be automatically rolled back
}
```

## Logging

The library supports custom logging through the `IStripeTransactionLogger` interface. By default, it uses a console logger, but you can implement your own logger:

```csharp
public class CustomLogger : IStripeTransactionLogger
{
    public void LogDebug(string message) { /* Your implementation */ }
    public void LogInformation(string message) { /* Your implementation */ }
    public void LogWarning(string message) { /* Your implementation */ }
    public void LogError(string message, Exception? exception = null) { /* Your implementation */ }
}

// Use custom logger
using (var transaction = new StripeTransaction(new CustomLogger()))
{
    // Your transaction code
}
```

## Supported Operations

The library supports rollback for the following Stripe operations:

- Customer creation/deletion
- Payment method attachment/detachment
- Subscription creation/cancellation
- Webhook endpoint creation/deletion
- Payment intent creation/cancellation
- Invoice creation/voiding

## Error Handling

The library automatically handles errors and performs rollbacks when operations fail:

```csharp
try
{
    using (var transaction = new StripeTransaction())
    {
        // Your transaction code
    }
}
catch (StripeException ex)
{
    // Handle Stripe-specific errors
}
catch (Exception ex)
{
    // Handle other errors
}
```

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## Dependencies

- .NET 6.0 or later
- Stripe.net 41.0.0 or later

## Support

For support, please open an issue in the GitHub repository. 