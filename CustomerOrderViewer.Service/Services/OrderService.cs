using CustomerOrderViewer.Interfaces;
using CustomerOrderViewer.Models;

namespace CustomerOrderViewer.Services
{
    public class OrderService : IOrderService
    {
        private IOrderRepository _orderRepository;

        public OrderService(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public async Task<List<Order>> GetOrdersForCustomerAsync(string customerIdInput)
        {
            var customerId = ParseCustomerId(customerIdInput);

            // Retrieve orders from the repository
            var orders = await _orderRepository.GetOrdersForCustomerAsync(customerId);

            return orders;
        }

        private static int ParseCustomerId(string customerIdInput)
        {
            // Validate and parse the input
            if (!int.TryParse(customerIdInput, out var customerId))
            {
                throw new ArgumentException("Invalid input. Please enter a valid customer ID.");
            }

            if (customerId <= 0)
            {
                throw new ArgumentException("Customer ID must be greater than 0.");
            }

            return customerId;
        }
    }
}
