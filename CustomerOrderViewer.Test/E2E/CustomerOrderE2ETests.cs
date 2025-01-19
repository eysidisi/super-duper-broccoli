using CustomerOrderViewer.Interfaces;
using CustomerOrderViewer.Models;
using CustomerOrderViewer.Repositories;
using CustomerOrderViewer.Services;
using CustomerOrderViewer.Test.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace CustomerOrderViewer.Test.E2E
{
    public class CustomerOrderE2ETests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly IOrderRepository _orderRepository;
        private readonly OrderService _orderService;

        public CustomerOrderE2ETests()
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json")
                .Build();

            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlServer(configuration.GetConnectionString("Test"))
                .Options;

            _context = new ApplicationDbContext(options);
            _orderRepository = new OrderRepository(_context);
            _orderService = new OrderService(_orderRepository);

            _context.Database.EnsureDeleted();
            _context.Database.EnsureCreated();

            // Use helper to seed data
            SeedDatabase(_context);
        }

        private void SeedDatabase(ApplicationDbContext context)
        {
            // Create test customers
            var customer1 = TestDataHelper.CreateCustomer(context, "John Doe");
            var customer2 = TestDataHelper.CreateCustomer(context, "Jane Smith");

            // Create test items
            var items = TestDataHelper.CreateItems(context, "Laptop", "Smartphone", "Headphones");

            // Create test orders and associate items
            TestDataHelper.CreateOrder(context, customer1, items.Take(2).ToList()); // Laptop, Smartphone for John Doe
            TestDataHelper.CreateOrder(context, customer2, new List<Item> { items[0], items[2] }); // Laptop, Headphones for Jane Smith
        }

        [Fact]
        public async Task GetOrdersForCustomer_ReturnsOrders_WithCustomerAndItems()
        {
            // Arrange
            var customerId = "1";

            // Act
            var orders = await _orderService.GetOrdersForCustomerAsync(customerId);

            // Assert
            Assert.NotEmpty(orders);

            var order1 = orders.FirstOrDefault(o => o.OrderId == 1);
            Assert.NotNull(order1);
            Assert.NotNull(order1.Items);
            Assert.Equal(2, order1.Items.Count);

            var itemNamesOrder1 = order1.Items.Select(i => i.Name).ToList();
            Assert.Contains("Laptop", itemNamesOrder1);
            Assert.Contains("Smartphone", itemNamesOrder1);

            // Second customer
            customerId = "2";
            orders = await _orderService.GetOrdersForCustomerAsync(customerId);

            Assert.NotEmpty(orders);

            var order2 = orders.FirstOrDefault(o => o.OrderId == 2);
            Assert.NotNull(order2);
            Assert.NotNull(order2.Items);
            Assert.Equal(2, order2.Items.Count);

            var itemNamesOrder2 = order2.Items.Select(i => i.Name).ToList();
            Assert.Contains("Laptop", itemNamesOrder2);
            Assert.Contains("Headphones", itemNamesOrder2);
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}
