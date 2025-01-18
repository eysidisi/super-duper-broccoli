using CustomerOrderViewer.Models;

namespace CustomerOrderViewer.Services
{
    public interface IOrderService
    {
        Task<List<Order>> GetOrdersForCustomerAsync(string customerId);
    }
}