using CustomerOrderViewer.Models;
using CustomerOrderViewer.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace CustomerOrderViewer.Infrastructure
{
    public static class ServiceExtensions
    {
        /// <summary>
        /// Ensures the database is created and all pending migrations are applied.
        /// </summary>
        /// <param name="serviceProvider">The service provider for dependency injection.</param>
        public static void EnsureDatabaseMigrated(this IServiceProvider serviceProvider)
        {
            // Create a new scope for services
            using var scope = serviceProvider.CreateScope();

            // Resolve the application's DbContext
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            // Apply migrations to ensure the database schema matches the current model
            dbContext.Database.Migrate();

            // Optional: Seed initial data (if needed)
            SeedData(dbContext);
        }

        private static void SeedData(ApplicationDbContext dbContext)
        {
            // Ensure the database is created
            dbContext.Database.EnsureCreated();

            // Seed Customers
            if (!dbContext.Customers.Any())
            {
                dbContext.Customers.AddRange(
                    new Customer { Name = "Default Customer" },
                    new Customer { Name = "John Doe" }
                );
                dbContext.SaveChanges(); // Save changes to generate CustomerIds
            }

            // Seed Items
            if (!dbContext.Items.Any())
            {
                dbContext.Items.AddRange(
                    new Item { Name = "Item A" },
                    new Item { Name = "Item B" }
                );
                dbContext.SaveChanges(); // Save changes to generate ItemIds
            }

            // Seed Orders
            if (!dbContext.Orders.Any())
            {
                var customer1 = dbContext.Customers.FirstOrDefault(c => c.Name == "Default Customer");
                var customer2 = dbContext.Customers.FirstOrDefault(c => c.Name == "John Doe");

                if (customer1 != null && customer2 != null)
                {
                    dbContext.Orders.AddRange(
                        new Order { CustomerId = customer1.CustomerId },
                        new Order { CustomerId = customer2.CustomerId }
                    );
                    dbContext.SaveChanges(); // Save changes to generate OrderIds
                }
            }

            // Seed ItemOrders
            if (!dbContext.ItemOrders.Any())
            {
                var customer1 = dbContext.Customers.FirstOrDefault(c => c.Name == "Default Customer");
                var customer2 = dbContext.Customers.FirstOrDefault(c => c.Name == "John Doe");
                var order1 = customer1 != null ? dbContext.Orders.FirstOrDefault(o => o.CustomerId == customer1.CustomerId) : null;
                var order2 = customer2 != null ? dbContext.Orders.FirstOrDefault(o => o.CustomerId == customer2.CustomerId) : null;

                var item1 = dbContext.Items.FirstOrDefault(i => i.Name == "Item A");
                var item2 = dbContext.Items.FirstOrDefault(i => i.Name == "Item B");

                if (order1 != null && order2 != null && item1 != null && item2 != null)
                {
                    dbContext.ItemOrders.AddRange(
                        new ItemOrder { OrderId = order1.OrderId, ItemId = item1.ItemId },
                        new ItemOrder { OrderId = order1.OrderId, ItemId = item2.ItemId },
                        new ItemOrder { OrderId = order2.OrderId, ItemId = item1.ItemId }
                    );
                    dbContext.SaveChanges();
                }
            }
        }
    }
}
