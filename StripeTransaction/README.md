# StripeTransaction

A .NET library that provides transaction support for Stripe operations with automatic rollback capabilities.

## Features

- Transaction support for Stripe operations
- Automatic rollback on failure
- Support for multiple operations in a single transaction
- Simple and intuitive API
- Built-in configuration management

## Installation

```bash
dotnet add package StripeTransaction
```

## Configuration

Initialize the package with your Stripe API key in your application startup:

```csharp
// In Program.cs or Startup.cs
StripeTransactionConfiguration.Initialize("your_stripe_api_key");

// Or with a specific API version
StripeTransactionConfiguration.Initialize("your_stripe_api_key", "2023-10-16");
```

## Usage

Here's a simple example of how to use the library:

```csharp
using (var transaction = new StripeTransaction())
{
    try
    {
        // Create a customer
        var customer = await transaction.ExecuteAsync(
            // Operation to execute
            async () =>
            {
                var service = new CustomerService();
                return await service.CreateAsync(new CustomerCreateOptions
                {
                    Email = "customer@example.com",
                    Name = "John Doe"
                });
            },
            // Rollback operation
            async (Customer c) =>
            {
                var service = new CustomerService();
                await service.DeleteAsync(c.Id);
            }
        );

        // Update the customer
        await transaction.ExecuteAsync(
            async () =>
            {
                var service = new CustomerService();
                await service.UpdateAsync(customer.Id, new CustomerUpdateOptions
                {
                    Description = "Updated customer"
                });
            },
            async () =>
            {
                var service = new CustomerService();
                await service.UpdateAsync(customer.Id, new CustomerUpdateOptions
                {
                    Description = "Original description"
                });
            }
        );
    }
    catch (Exception ex)
    {
        // If any operation fails, all previous operations are automatically rolled back
        Console.WriteLine($"Transaction failed: {ex.Message}");
    }
}
```

## How It Works

1. Create a transaction scope using `using (var transaction = new StripeTransaction())`
2. Inside the scope, use `ExecuteAsync` to perform Stripe operations
3. For each operation, provide:
   - The operation to execute
   - The rollback operation to perform if something fails
4. If any operation fails, all previous operations are automatically rolled back

## Best Practices

1. Always initialize the package with your Stripe API key at application startup
2. Use the `using` statement to ensure proper disposal and rollback
3. Keep rollback operations simple and focused
4. Handle exceptions appropriately
5. Test your rollback scenarios thoroughly

## License

MIT License 