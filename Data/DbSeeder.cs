using EfCoreProjectionDemo.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace EfCoreProjectionDemo.Data;

public static class DbSeeder
{
    public static async Task SeedAsync(AppDbContext context)
    {
        if (await context.Categories.AnyAsync()) return;

        var categories = new List<Category>
        {
            new() { Name = "Electronics" },
            new() { Name = "Books" },
            new() { Name = "Clothing" }
        };

        await context.Categories.AddRangeAsync(categories);

        var products = new List<Product>
        {
            new() { Name = "Laptop", Price = 1200.00m, Category = categories[0] },
            new() { Name = "Smartphone", Price = 800.00m, Category = categories[0] },
            new() { Name = "C# in Depth", Price = 45.00m, Category = categories[1] },
            new() { Name = "Entity Framework Core", Price = 50.00m, Category = categories[1] },
            new() { Name = "T-Shirt", Price = 25.00m, Category = categories[2] }
        };

        await context.Products.AddRangeAsync(products);

        var customers = new List<Customer>
        {
            new() { FirstName = "John", LastName = "Doe", Email = "john.doe@example.com" },
            new() { FirstName = "Jane", LastName = "Smith", Email = "jane.smith@example.com" }
        };

        await context.Customers.AddRangeAsync(customers);

        var orders = new List<Order>
        {
            new() 
            { 
                Customer = customers[0], 
                OrderDate = DateTime.UtcNow.AddDays(-5),
                OrderItems = new List<OrderItem>
                {
                    new() { Product = products[0], Quantity = 1, UnitPrice = products[0].Price },
                    new() { Product = products[2], Quantity = 2, UnitPrice = products[2].Price }
                }
            },
            new() 
            { 
                Customer = customers[1], 
                OrderDate = DateTime.UtcNow.AddDays(-2),
                OrderItems = new List<OrderItem>
                {
                    new() { Product = products[1], Quantity = 1, UnitPrice = products[1].Price },
                    new() { Product = products[4], Quantity = 3, UnitPrice = products[4].Price }
                }
            }
        };

        await context.Orders.AddRangeAsync(orders);
        await context.SaveChangesAsync();
    }
}
