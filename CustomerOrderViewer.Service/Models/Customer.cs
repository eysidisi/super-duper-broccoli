using System.Collections.Generic;
using System.Linq;

namespace CustomerOrderViewer.Models
{
    public class Customer
    {
        public int CustomerId { get; set; }
        public string Name { get; set; }

        // Navigation property for the related orders
        public ICollection<Order> Orders { get; set; }
    }

    public class Order
    {
        public int OrderId { get; set; }
        public int CustomerId { get; set; }
        public Customer Customer { get; set; }

        public ICollection<ItemOrder> ItemOrders { get; set; } = new List<ItemOrder>(); // Join table for many-to-many
        public ICollection<Item> Items => ItemOrders.Select(io => io.Item).ToList(); // Navigation property for items
    }

    public class Item
    {
        public int ItemId { get; set; }
        public string Name { get; set; }

        public ICollection<ItemOrder> ItemOrders { get; set; } = new List<ItemOrder>(); // Navigation property for many-to-many
    }

    public class ItemOrder
    {
        public int OrderId { get; set; }
        public Order Order { get; set; }

        public int ItemId { get; set; }
        public Item Item { get; set; }
    }

}
