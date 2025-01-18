using CustomerOrderViewer.Infrastructure;
using CustomerOrderViewer.Services;
using Microsoft.Extensions.DependencyInjection;

public class Program
{
    public static async Task Main(string[] args)
    {
        // Set the environment (could be "Prod" or "Test")
        string environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Test";

        // Setup Dependency Injection and Configuration
        var serviceProvider = ServiceConfiguration.ConfigureServices(environment);

        // Handle user input and display order information
        await RunAppAsync(serviceProvider);
    }

    public static async Task RunAppAsync(IServiceProvider serviceProvider)
    {
        var orderService = serviceProvider.GetRequiredService<IOrderService>();
        Console.WriteLine("Enter customer ID:");
        var customerId = Console.ReadLine();

        try
        {
            var orders = await orderService.GetOrdersForCustomerAsync(customerId);

            if (orders == null || !orders.Any())
            {
                Console.WriteLine($"No orders found for the customer with ID {customerId}.");
                return;
            }

            Console.WriteLine($"The orders for the user with ID {customerId} are these:");
            foreach (var order in orders)
            {
                Console.WriteLine($"Order ID: {order.OrderId}");
                foreach (var item in order.Items)
                {
                    Console.WriteLine($"Item: {item.Name}");
                }
            }
        }
        catch (ArgumentException ex)
        {
            Console.WriteLine($"Input error: {ex.Message}");
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
}
