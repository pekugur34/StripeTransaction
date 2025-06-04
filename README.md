# StripeTransaction

A .NET library that provides transaction-like behavior for Stripe operations, ensuring atomicity and rollback capabilities for complex Stripe operations.

[![NuGet Version](https://img.shields.io/nuget/v/StripeTransaction.svg)](https://www.nuget.org/packages/StripeTransaction)
[![NuGet Downloads](https://img.shields.io/nuget/dt/StripeTransaction.svg)](https://www.nuget.org/packages/StripeTransaction)

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
- 🔗 Support for executing multiple operations in a single transaction

## Installation

```bash
dotnet add package StripeTransaction
```

## Requirements

- .NET Core 3.1 or later (.NET Core 3.1, .NET 5, .NET 6, .NET 7, .NET 8)
- Stripe.net 41.0.0 or later

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
### Multiple Operations in a Single Transaction

You can also execute multiple operations in a single `ExecuteAsync` call:

```csharp
using (var transaction = new StripeTransaction())
{
    // Create a customer and attach a payment method in one transaction
    var (customer, paymentMethod) = await transaction.ExecuteAsync(async () =>
    {
        // Create customer
        var customerService = new CustomerService();
        var customer = await customerService.CreateAsync(new CustomerCreateOptions
        {
            Email = "customer@example.com",
            Name = "John Doe"
        });

        // Create payment method
        var paymentMethodService = new PaymentMethodService();
        var paymentMethod = await paymentMethodService.CreateAsync(new PaymentMethodCreateOptions
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

        // Attach payment method to customer
        await paymentMethodService.AttachAsync(paymentMethod.Id, new PaymentMethodAttachOptions
        {
            Customer = customer.Id
        });

        // Return both objects
        return (customer, paymentMethod);
    });

    // Use the customer and payment method
    Console.WriteLine($"Created customer: {customer.Id}");
    Console.WriteLine($"Attached payment method: {paymentMethod.Id}");
}
```

### Complex Transaction Example

Here's an example of a more complex transaction with multiple operations:

```csharp
using (var transaction = new StripeTransaction())
{
    var (customer, subscription, paymentIntent) = await transaction.ExecuteAsync(async () =>
    {
        // 1. Create customer
        var customerService = new CustomerService();
        var customer = await customerService.CreateAsync(new CustomerCreateOptions
        {
            Email = "customer@example.com",
            Name = "John Doe"
        });

        // 2. Create and attach payment method
        var paymentMethodService = new PaymentMethodService();
        var paymentMethod = await paymentMethodService.CreateAsync(new PaymentMethodCreateOptions
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
        await paymentMethodService.AttachAsync(paymentMethod.Id, new PaymentMethodAttachOptions
        {
            Customer = customer.Id
        });

        // 3. Create subscription
        var subscriptionService = new SubscriptionService();
        var subscription = await subscriptionService.CreateAsync(new SubscriptionCreateOptions
        {
            Customer = customer.Id,
            Items = new List<SubscriptionItemOptions>
            {
                new SubscriptionItemOptions
                {
                    Price = "price_H5ggYwtDq4fbrJ"
                }
            }
        });

        // 4. Create payment intent
        var paymentIntentService = new PaymentIntentService();
        var paymentIntent = await paymentIntentService.CreateAsync(new PaymentIntentCreateOptions
        {
            Amount = 2000,
            Currency = "usd",
            Customer = customer.Id,
            PaymentMethod = paymentMethod.Id,
            Confirm = true
        });

        // Return all created objects
        return (customer, subscription, paymentIntent);
    });

    // Use the created objects
    Console.WriteLine($"Created customer: {customer.Id}");
    Console.WriteLine($"Created subscription: {subscription.Id}");
    Console.WriteLine($"Created payment intent: {paymentIntent.Id}");
}
```

## Advanced Usage

### Custom Logging

The library supports custom logging through the `IStripeTransactionLogger` interface:

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
### Error Handling

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

### Supported Operations

The library supports rollback for the following Stripe operations:

- Customer creation/deletion
- Payment method attachment/detachment
- Subscription creation/cancellation
- Webhook endpoint creation/deletion
- Payment intent creation/cancellation
- Invoice creation/voiding

## Best Practices

1. Always initialize the library with your Stripe API key at application startup
2. Use the `using` statement to ensure proper disposal and rollback
3. Implement proper error handling
4. Use custom logging for better debugging
5. Test your rollback scenarios thoroughly
6. Group related operations in a single `ExecuteAsync` call for better atomicity

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request. For major changes, please open an issue first to discuss what you would like to change.

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## Support

- 📚 [Documentation](https://github.com/pekugur34/StripeTransaction)
- 🐛 [Issue Tracker](https://github.com/pekugur34/StripeTransaction/issues)
- 💬 [Discussions](https://github.com/pekugur34/StripeTransaction/discussions)

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request. 
## Version History

- 1.0.0
  - Initial release
  - Support for basic Stripe operations
  - Automatic rollback functionality
  - Custom logging support
  - Support for multiple operations in a single transaction 
