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

// Modern using declaration (C# 8.0+)
using var transaction = new StripeTransaction();

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

// Traditional using statement (all C# versions)
using (var transaction = new StripeTransaction())
{
    var customer = await transaction.ExecuteAsync(async () =>
    {
        var customerService = new CustomerService();
        return await customerService.CreateAsync(new CustomerCreateOptions
        {
            Email = "customer@example.com",
            Name = "John Doe"
        });
    });
}
```

## Supported Operations

### Customer Operations
```csharp
// Modern using declaration
using var transaction = new StripeTransaction();

// Create customer
var customer = await transaction.ExecuteAsync(async () =>
{
    var service = new CustomerService();
    return await service.CreateAsync(new CustomerCreateOptions
    {
        Email = "customer@example.com",
        Name = "John Doe"
    });
});

// Update customer
await transaction.ExecuteAsync(async () =>
{
    var service = new CustomerService();
    await service.UpdateAsync(customer.Id, new CustomerUpdateOptions
    {
        Description = "Updated customer"
    });
});
```

### Payment Method Operations
```csharp
using var transaction = new StripeTransaction();

// Create and attach payment method
var (paymentMethod, customer) = await transaction.ExecuteAsync(async () =>
{
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

    return (paymentMethod, customer);
});
```

### Subscription Operations
```csharp
using var transaction = new StripeTransaction();

// Create subscription
var subscription = await transaction.ExecuteAsync(async () =>
{
    var service = new SubscriptionService();
    return await service.CreateAsync(new SubscriptionCreateOptions
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
});
```

### Payment Intent Operations
```csharp
using var transaction = new StripeTransaction();

// Create and confirm payment intent
var paymentIntent = await transaction.ExecuteAsync(async () =>
{
    var service = new PaymentIntentService();
    return await service.CreateAsync(new PaymentIntentCreateOptions
    {
        Amount = 2000,
        Currency = "usd",
        Customer = customer.Id,
        PaymentMethod = paymentMethod.Id,
        Confirm = true
    });
});
```

### Webhook Operations
```csharp
using var transaction = new StripeTransaction();

// Create webhook endpoint
var webhook = await transaction.ExecuteAsync(async () =>
{
    var service = new WebhookEndpointService();
    return await service.CreateAsync(new WebhookEndpointCreateOptions
    {
        Url = "https://your-domain.com/webhook",
        EnabledEvents = new List<string> { "payment_intent.succeeded" }
    });
});
```

### Complex Transaction Example
```csharp
using var transaction = new StripeTransaction();

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

    return (customer, subscription, paymentIntent);
});
```

## Error Handling

```csharp
try
{
    using var transaction = new StripeTransaction();
    // Your transaction code
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

## Custom Logging

```csharp
public class CustomLogger : IStripeTransactionLogger
{
    public void LogDebug(string message) { /* Your implementation */ }
    public void LogInformation(string message) { /* Your implementation */ }
    public void LogWarning(string message) { /* Your implementation */ }
    public void LogError(string message, Exception? exception = null) { /* Your implementation */ }
}

// Use custom logger
using var transaction = new StripeTransaction(new CustomLogger());
// Your transaction code
```

## Best Practices

1. Always initialize the library with your Stripe API key at application startup
2. Use the `using` declaration for cleaner code (C# 8.0+)
3. Implement proper error handling
4. Use custom logging for better debugging
5. Test your rollback scenarios thoroughly
6. Group related operations in a single `ExecuteAsync` call for better atomicity

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## Support

- 📚 [Documentation](https://github.com/pekugur34/StripeTransaction)
- 🐛 [Issue Tracker](https://github.com/pekugur34/StripeTransaction/issues)
- 💬 [Discussions](https://github.com/pekugur34/StripeTransaction/discussions)

## Version History

- 1.0.0
  - Initial release
  - Support for basic Stripe operations
  - Automatic rollback functionality
  - Custom logging support
  - Support for multiple operations in a single transaction
