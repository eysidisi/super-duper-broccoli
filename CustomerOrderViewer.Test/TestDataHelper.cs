using System.Collections.Generic;
using System.Linq;
using CustomerOrderViewer.Models;
using CustomerOrderViewer.Repositories;
using Microsoft.EntityFrameworkCore;

namespace CustomerOrderViewer.Test.Helpers
{
    public static class TestDataHelper
    {
        public static Customer CreateCustomer(ApplicationDbContext context, string name)
        {
            var customer = new Customer { Name = name };
            context.Customers.Add(customer);
            context.SaveChanges();
            return customer;
        }

        public static List<Item> CreateItems(ApplicationDbContext context, params string[] itemNames)
        {
            var items = itemNames.Select(name => new Item { Name = name }).ToList();
            context.Items.AddRange(items);
            context.SaveChanges();
            return items;
        }

        public static Order CreateOrder(ApplicationDbContext context, Customer customer, List<Item> items)
        {
            var order = new Order { Customer = customer };
            context.Orders.Add(order);
            context.SaveChanges();

            var itemOrders = items.Select(item => new ItemOrder { Order = order, Item = item }).ToList();
            context.ItemOrders.AddRange(itemOrders);
            context.SaveChanges();

            return order;
        }

    }
}
