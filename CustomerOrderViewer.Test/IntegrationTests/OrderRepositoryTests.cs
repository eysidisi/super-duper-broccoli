using CustomerOrderViewer.Interfaces;
using CustomerOrderViewer.Models;
using CustomerOrderViewer.Repositories;
using CustomerOrderViewer.Test.Helpers;
using Microsoft.EntityFrameworkCore;

namespace CustomerOrderViewer.Test.IntegrationTests
{
    public class OrderRepositoryTests : IDisposable
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ApplicationDbContext _context;

        public OrderRepositoryTests()
        {
            // Generate a unique database name for each test
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: $"CustomerOrderTestDB_{Guid.NewGuid()}")
                .Options;

            _context = new ApplicationDbContext(options);
            _orderRepository = new OrderRepository(_context);
        }

        [Fact]
        public async Task GetOrdersForCustomer_ReturnsOrders_WithCustomerAndItemDetails()
        {
            // Arrange
            var customer = TestDataHelper.CreateCustomer(_context, "John Doe");
            var items = TestDataHelper.CreateItems(_context, "Laptop", "Mouse");
            var order = TestDataHelper.CreateOrder(_context, customer, items);

            // Act
            var orders = await _orderRepository.GetOrdersForCustomerAsync(customer.CustomerId);

            // Assert
            AssertOrderDetails(orders, 1, "Laptop", "Mouse");
        }

        [Fact]
        public async Task GetOrdersForCustomer_ReturnsEmptyList_WhenCustomerHasNoOrders()
        {
            // Arrange
            var customer = TestDataHelper.CreateCustomer(_context, "John Doe");

            // Act
            var orders = await _orderRepository.GetOrdersForCustomerAsync(customer.CustomerId);

            // Assert
            Assert.Empty(orders);
        }

        [Fact]
        public async Task GetOrdersForCustomer_ReturnsEmptyList_WhenCustomerDoesntExist()
        {
            // Arrange
            var customer = TestDataHelper.CreateCustomer(_context, "John Doe");
            var items = TestDataHelper.CreateItems(_context, "Laptop", "Mouse");
            TestDataHelper.CreateOrder(_context, customer, items);

            // Act
            var orders = await _orderRepository.GetOrdersForCustomerAsync(2);

            // Assert
            Assert.Empty(orders);
        }

        private void AssertOrderDetails(List<Order> orders, int expectedOrderCount, params string[] expectedItemNames)
        {
            Assert.NotNull(orders);
            Assert.Equal(expectedOrderCount, orders.Count);

            var actualItemNames = orders.SelectMany(o => o.Items).Select(i => i.Name).ToList();
            foreach (var expectedItemName in expectedItemNames)
            {
                Assert.Contains(expectedItemName, actualItemNames);
            }
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}
