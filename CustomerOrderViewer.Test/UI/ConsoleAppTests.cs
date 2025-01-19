using CustomerOrderViewer.Interfaces;
using CustomerOrderViewer.Models;
using CustomerOrderViewer.Services;
using Moq;

namespace UI
{
    public class ConsoleAppTests
    {
        private readonly Mock<IOrderRepository> _mockOrderRepository;
        private readonly Mock<IServiceProvider> _mockServiceProvider;
        private readonly IOrderService _orderService;

        public ConsoleAppTests()
        {
            // Setup a mock repository
            _mockOrderRepository = new Mock<IOrderRepository>();

            // Setup the OrderService to use the mock repository
            _orderService = new OrderService(_mockOrderRepository.Object);

            // Setup a mock service provider
            _mockServiceProvider = new Mock<IServiceProvider>();

            // Configure the service provider to return the OrderService
            _mockServiceProvider
                .Setup(sp => sp.GetService(typeof(IOrderService)))
                .Returns(_orderService);

            // Seed the mock repository with test data
            SeedMockRepository();
        }

        private void SeedMockRepository()
        {
            // Create mock data
            var customerId = 1;

            var item1 = new Item { ItemId = 1, Name = "Laptop" };
            var item2 = new Item { ItemId = 2, Name = "Phone" };

            // Create order data with related items through the ItemOrder join table
            var order1 = new Order
            {
                OrderId = 1,
                CustomerId = customerId,
                ItemOrders = new List<ItemOrder>
            {
                new ItemOrder { ItemId = item1.ItemId, Item = item1 }
            }
            };

            var order2 = new Order
            {
                OrderId = 2,
                CustomerId = customerId,
                ItemOrders = new List<ItemOrder>
            {
                new ItemOrder { ItemId = item2.ItemId, Item = item2 }
            }
            };

            var orders = new List<Order> { order1, order2 };

            // Add the customer and items
            var customer = new Customer { CustomerId = customerId, Name = "John Doe" };

            // Setup the mock to return orders for a given customer ID
            _mockOrderRepository
                .Setup(repo => repo.GetOrdersForCustomerAsync(customerId))
                .ReturnsAsync(orders);
        }

        [Fact]
        public async Task TestGetOrdersForUser_PromptsForCustomerId_AndReturnsCorrectOrderDetails()
        {
            // Arrange: Capture console output and simulate console input
            var output = new StringWriter();
            Console.SetOut(output);

            var input = new StringReader("1\nexit\n");
            Console.SetIn(input);

            // Act: Call the application logic
            await Program.RunAppAsync(_mockServiceProvider.Object);

            // Assert: Verify the output contains the expected details
            var result = output.ToString();

            Assert.Contains("Enter customer ID (or type 'exit' to quit):", result);
            Assert.Contains("The orders for the user with ID 1 are:", result);
            Assert.Contains("Order ID: 1", result);
            Assert.Contains("Item: Laptop", result);
            Assert.Contains("Order ID: 2", result);
            Assert.Contains("Item: Phone", result);

            // Verify that the correct method was called on the mock repository
            _mockOrderRepository.Verify(repo => repo.GetOrdersForCustomerAsync(1), Times.Once);
        }

        [Fact]
        public async Task GetOrdersForCustomer_ReturnsEmptyList_WhenCustomerHasNoOrders()
        {
            // Arrange: Capture console output and simulate console input
            var customerId = "2";
            var output = new StringWriter();
            Console.SetOut(output);

            var input = new StringReader($"{customerId}\nexit\n");
            Console.SetIn(input);

            // Setup the mock to return an empty list for a given customer ID
            _mockOrderRepository
                .Setup(repo => repo.GetOrdersForCustomerAsync(int.Parse(customerId)))
                .ReturnsAsync(new List<Order>());

            // Act: Call the application logic
            await Program.RunAppAsync(_mockServiceProvider.Object);

            // Assert: Verify the output contains the expected details
            var result = output.ToString();

            Assert.Contains($"No orders found for the customer with ID {customerId}.", result);
            Assert.DoesNotContain("Order ID: 1", result);
            Assert.DoesNotContain("Item: Laptop", result);
            Assert.DoesNotContain("Order ID: 2", result);
            Assert.DoesNotContain("Item: Phone", result);
        }
    }
}