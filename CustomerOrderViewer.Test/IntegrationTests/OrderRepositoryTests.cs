using CustomerOrderViewer.Interfaces;
using CustomerOrderViewer.Models;
using CustomerOrderViewer.Repositories;
using Microsoft.EntityFrameworkCore;

namespace CustomerOrderViewer.Test.IntegrationTests
{
    public class OrderRepositoryTests : IDisposable
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ApplicationDbContext _context;

        public OrderRepositoryTests()
        {
            // Setup in-memory database
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "CustomerOrderTestDB")
                .Options;

            _context = new ApplicationDbContext(options);
            _orderRepository = new OrderRepository(_context);
        }

        [Fact]
        public async Task GetOrdersForCustomer_ReturnsOrders_WithCustomerAndItemDetails()
        {
            // Seed data
            var customer1 = new Customer { CustomerId = 1, Name = "John Doe" };
            var item1 = new Item { ItemId = 1, Name = "Laptop" };
            var item2 = new Item { ItemId = 2, Name = "Mouse" };

            // Add the customer and items to the in-memory database
            _context.Customers.Add(customer1);
            _context.Items.AddRange(item1, item2);
            await _context.SaveChangesAsync(); // Save to generate IDs for the customer and items

            // Create the order and link it to the customer
            var order1 = new Order
            {
                CustomerId = customer1.CustomerId, // Use the auto-generated CustomerId
            };

            // Add the order to the in-memory database and save to generate the OrderId
            _context.Orders.Add(order1);
            await _context.SaveChangesAsync(); // Save to generate the OrderId

            // Now that OrderId is generated, create the ItemOrder entries
            var itemOrder1 = new ItemOrder { OrderId = order1.OrderId, ItemId = item1.ItemId };
            var itemOrder2 = new ItemOrder { OrderId = order1.OrderId, ItemId = item2.ItemId };

            // Add ItemOrder links to the database
            _context.ItemOrders.AddRange(itemOrder1, itemOrder2);
            await _context.SaveChangesAsync(); // Persist the ItemOrder relationships

            // Arrange: Set up customer ID to fetch orders
            var customerId = 1;

            // Act: Fetch the orders for this customer
            var orders = await _orderRepository.GetOrdersForCustomerAsync(customerId);

            // Assert: Verify that the orders are returned with the correct customer and item details
            Assert.NotNull(orders);
            Assert.Single(orders); // Customer 1 has 1 order

            // Verify that the order contains the correct items
            Assert.Contains(orders[0].Items, i => i.Name == "Laptop");
            Assert.Contains(orders[0].Items, i => i.Name == "Mouse");
        }

        [Fact]
        public async Task GetOrdersForCustomer_ReturnsEmptyList_WhenCustomerHasNoOrders()
        {
            // Arrange: Set up mock data for a customer with no orders
            var customerId = 1;
            var customer1 = new Customer { CustomerId = customerId, Name = "John Doe" };
            _context.Customers.Add(customer1);
            _context.SaveChanges();

            // Act: Fetch the orders for this customer
            var orders = await _orderRepository.GetOrdersForCustomerAsync(customerId);

            // Assert: Verify that the result is an empty list
            Assert.Empty(orders);
        }

        public void Dispose()
        {
            // Clear the in-memory database after each test
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}
