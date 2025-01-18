using CustomerOrderViewer.Interfaces;
using CustomerOrderViewer.Models;
using CustomerOrderViewer.Repositories;
using CustomerOrderViewer.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CustomerOrderViewer.Tests.E2E
{
    public class CustomerOrderE2ETests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly IOrderRepository _orderRepository;
        private readonly OrderService _orderService;

        public CustomerOrderE2ETests()
        {
            // Setup configuration for testing
            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json")
                .Build();

            // Configure MSSQL Server connection string for testing
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlServer(configuration.GetConnectionString("Test"))
                .Options;

            // Initialize DbContext, Repository, and Service
            _context = new ApplicationDbContext(options);
            _orderRepository = new OrderRepository(_context);
            _orderService = new OrderService(_orderRepository);

            // Ensure database is clean before starting
            _context.Database.EnsureDeleted();
            _context.Database.EnsureCreated();

            // Seed the database
            SeedDatabase();
        }

        private void SeedDatabase()
        {
            // Create some test customers
            var customer1 = new Customer { Name = "John Doe" };
            var customer2 = new Customer { Name = "Jane Smith" };

            // Create some test items
            var item1 = new Item { Name = "Laptop" };
            var item2 = new Item { Name = "Smartphone" };
            var item3 = new Item { Name = "Headphones" };

            // Add customers and items to the context
            _context.Customers.AddRange(customer1, customer2);
            _context.Items.AddRange(item1, item2, item3);

            // Create some test orders
            var order1 = new Order { Customer = customer1 };
            var order2 = new Order { Customer = customer2 };

            // Add orders to the context
            _context.Orders.AddRange(order1, order2);

            // Create some test item orders (many-to-many relationships)
            var itemOrder1 = new ItemOrder { Order = order1, Item = item1 };
            var itemOrder2 = new ItemOrder { Order = order1, Item = item2 };
            var itemOrder3 = new ItemOrder { Order = order2, Item = item3 };
            var itemOrder4 = new ItemOrder { Order = order2, Item = item1 };

            // Add item orders to the context (this will populate the many-to-many join table)
            _context.ItemOrders.AddRange(itemOrder1, itemOrder2, itemOrder3, itemOrder4);

            // Save all changes to the database
            _context.SaveChanges();
        }

        [Fact]
        public async Task GetOrdersForCustomer_ReturnsOrders_WithCustomerAndItems()
        {
            // Arrange
            var customerId = "1"; // Test for customer with ID 1 (John Doe)

            // Act: Use the service to retrieve orders for the customer
            var orders = await _orderService.GetOrdersForCustomerAsync(customerId);

            // Assert: Validate the orders
            Assert.NotEmpty(orders);  // Assert that the orders are not empty.

            // Validate the first order for customer 1 (John Doe)
            var order1 = orders.FirstOrDefault(o => o.OrderId == 1);
            Assert.NotNull(order1);
            Assert.NotNull(order1.Items);
            Assert.Equal(2, order1.Items.Count);  // Check that there are exactly two items in the order

            var itemNamesOrder1 = order1.Items.Select(i => i.Name).ToList();
            Assert.Contains("Laptop", itemNamesOrder1);
            Assert.Contains("Smartphone", itemNamesOrder1);

            // Assert that the second customer has their correct orders and items
            customerId = "2"; // Test for customer with ID 2 (Jane Smith)

            orders = await _orderService.GetOrdersForCustomerAsync(customerId);

            // Assert: Validate the orders for customer 2
            Assert.NotEmpty(orders);  // Assert that the orders are not empty.

            var order2 = orders.FirstOrDefault(o => o.OrderId == 2);
            Assert.NotNull(order2);
            Assert.NotNull(order2.Items);
            Assert.Equal(2, order2.Items.Count);  // Check that there are exactly two items in the order

            var itemNamesOrder3 = order2.Items.Select(i => i.Name).ToList();
            Assert.Contains("Laptop", itemNamesOrder3);
            Assert.Contains("Headphones", itemNamesOrder3);
        }


        public void Dispose()
        {
            // Clean up the database after tests
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}
