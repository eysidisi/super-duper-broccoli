using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using CustomerOrderViewer.Repositories;
using CustomerOrderViewer.Services;
using CustomerOrderViewer.Interfaces;
using System;

namespace CustomerOrderViewer.Infrastructure
{
    public class ServiceConfiguration
    {
        public static IServiceProvider ConfigureServices(string environment)
        {
            // Setup Dependency Injection and Configuration
            var serviceCollection = new ServiceCollection();

            // Load configuration from appsettings (and environment-specific settings)
            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)  // Ensure the correct path
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true) // Load base config
                .Build();

            // Get the connection string based on the environment parameter (either "Prod" or "Test")
            string connectionString = configuration.GetConnectionString(environment);

            // Register services and repositories
            serviceCollection.AddSingleton<IConfiguration>(configuration);
            serviceCollection.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));
            serviceCollection.AddScoped<IOrderRepository, OrderRepository>();
            serviceCollection.AddScoped<IOrderService, OrderService>();

            // Build and return service provider
            return serviceCollection.BuildServiceProvider();
        }
    }
}
