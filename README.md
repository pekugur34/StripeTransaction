# StripeTransaction

A .NET library that provides transaction-like behavior for Stripe operations, ensuring atomicity and rollback capabilities for complex Stripe operations.

[![NuGet Version](https://img.shields.io/nuget/v/StripeTransaction.svg)](https://www.nuget.org/packages/StripeTransaction)
[![NuGet Downloads](https://img.shields.io/nuget/dt/StripeTransaction.svg)](https://www.nuget.org/packages/StripeTransaction)

## Quick Start

```csharp
// Initialize with your Stripe API key
StripeTransactionConfiguration.Initialize("your_stripe_api_key");

// Use the transaction
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
}
```

## Features

- 🔄 Transaction-like behavior for Stripe operations
- ⚡ Automatic rollback on failure
- 📝 Comprehensive logging support
- 🛠️ Support for multiple Stripe operations:
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

## Requirements

- .NET Core 3.1 or later (.NET Core 3.1, .NET 5, .NET 6, .NET 7, .NET 8)
- Stripe.net 41.0.0 or later

## Documentation

For detailed documentation, examples, and API reference, visit our [GitHub repository](https://github.com/pekugur34/StripeTransaction).

## License

This project is licensed under the MIT License - see the [LICENSE](https://github.com/pekugur34/StripeTransaction/blob/main/LICENSE) file for details.

## Support

- 📚 [Documentation](https://github.com/pekugur34/StripeTransaction)
- 🐛 [Issue Tracker](https://github.com/pekugur34/StripeTransaction/issues)
- 💬 [Discussions](https://github.com/pekugur34/StripeTransaction/discussions)

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request. 
