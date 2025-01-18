using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CustomerOrderViewer.Interfaces;
using CustomerOrderViewer.Models;
using Microsoft.EntityFrameworkCore;

namespace CustomerOrderViewer.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly ApplicationDbContext _context;

        public OrderRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Order>> GetOrdersForCustomerAsync(int customerId)
        {
            var orders = await _context.Orders
                .Where(o => o.CustomerId == customerId)
                .Include(o => o.ItemOrders)  // Eagerly load the ItemOrders join table
                .ThenInclude(io => io.Item)  // Eagerly load the associated Item in the ItemOrder
                .ToListAsync();

            // Return the fetched orders
            return orders;
        }
    }
}
