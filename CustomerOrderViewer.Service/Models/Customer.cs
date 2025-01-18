using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace CustomerOrderViewer.Models
{
    public class Customer
    {
        public int CustomerId { get; set; }
        public string Name { get; set; }

        // Navigation property for the related orders
        public virtual ICollection<Order> Orders { get; set; }
    }

    public class Item
    {
        public int ItemId { get; set; }
        public string Name { get; set; }

        // Navigation property for the related ItemOrders (join table)
        public virtual ICollection<ItemOrder> ItemOrders { get; set; } = new List<ItemOrder>(); // Initialize to empty list

    }

    public class Order
    {
        public int OrderId { get; set; }
        public int CustomerId { get; set; }

        // Navigation property for the related customer
        public Customer Customer { get; set; }

        // Navigation property for the related ItemOrders (join table)
        public virtual ICollection<ItemOrder> ItemOrders { get; set; } = new List<ItemOrder>();

        // Non-mapped property for items
        [NotMapped]
        public ICollection<Item> Items
        {
            get
            {
                return ItemOrders?.Select(io => io.Item).ToList();
            }
        }
    }

    public class ItemOrder
    {
        public int ItemOrderId {  get; set; }
        public int OrderId { get; set; }
        public int ItemId { get; set; }

        // Navigation property for the related order
        public Order Order { get; set; }

        // Navigation property for the related item
        public Item Item { get; set; }
    }
}
