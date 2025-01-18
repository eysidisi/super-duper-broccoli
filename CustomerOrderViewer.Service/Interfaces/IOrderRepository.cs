using CustomerOrderViewer.Models;

namespace CustomerOrderViewer.Interfaces
{
    public interface IOrderRepository
    {
        Task<List<Order>> GetOrdersForCustomerAsync(int customerId);
    }
}