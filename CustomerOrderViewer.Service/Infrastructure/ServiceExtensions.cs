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

        /// <summary>
        /// Seeds the database with initial data if necessary.
        /// </summary>
        /// <param name="dbContext">The application's DbContext.</param>
        private static void SeedData(ApplicationDbContext dbContext)
        {
            // Example: Add a default customer if the Customers table is empty
            if (!dbContext.Customers.Any())
            {
                dbContext.Customers.Add(new Customer { CustomerId = 1, Name = "Default Customer" });
                dbContext.SaveChanges();
            }

            // You can add other seeding logic here as needed
        }
    }
}
