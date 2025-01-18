using CustomerOrderViewer.Interfaces;
using CustomerOrderViewer.Models;
using CustomerOrderViewer.Services;
using Moq;

namespace CustomerOrderViewer.Test.UnitTests
{
    public class OrderServiceTests
    {
        private readonly Mock<IOrderRepository> _orderRepositoryMock;
        private readonly OrderService _orderService;

        public OrderServiceTests()
        {
            // Initialize the mock repository
            _orderRepositoryMock = new Mock<IOrderRepository>();

            // Create an instance of the OrderService, injecting the mocked repository
            _orderService = new OrderService(_orderRepositoryMock.Object);
        }

        [Fact]
        public async Task GetOrdersForCustomer_ReturnsOrders_WhenCustomerHasOrders()
        {
            // Arrange: Set up mock data for a customer with ID 1
            var customerIdInt = 1;
            var customerId = "1";
            var expectedOrders = new List<Order>
            {
                new Order { OrderId = 1},
                new Order { OrderId = 2}
            };

            // Setup the mock repository to return the expected orders asynchronously
            _orderRepositoryMock.Setup(repo => repo.GetOrdersForCustomerAsync(customerIdInt))
                .ReturnsAsync(expectedOrders);

            // Act: Call the method we're testing
            var actualOrders = await _orderService.GetOrdersForCustomerAsync(customerId);

            // Assert: Verify that the result matches the expected orders
            Assert.NotNull(actualOrders);
            Assert.Equal(expectedOrders.Count, actualOrders.Count);
            Assert.Equal(expectedOrders[0].OrderId, actualOrders[0].OrderId);
            Assert.Equal(expectedOrders[1].OrderId, actualOrders[1].OrderId);
        }

        [Fact]
        public async Task GetOrdersForCustomer_ReturnsEmptyList_WhenCustomerHasNoOrders()
        {
            // Arrange: Set up mock data for a customer with no orders
            var customerIdInt = 2;
            var customerId = "2";
            var expectedOrders = new List<Order>();

            // Setup the mock repository to return an empty list asynchronously
            _orderRepositoryMock.Setup(repo => repo.GetOrdersForCustomerAsync(customerIdInt))
                .ReturnsAsync(expectedOrders);

            // Act: Call the method we're testing
            var actualOrders = await _orderService.GetOrdersForCustomerAsync(customerId);

            // Assert: Verify that the result is an empty list
            Assert.Empty(actualOrders);
        }
    }
}
