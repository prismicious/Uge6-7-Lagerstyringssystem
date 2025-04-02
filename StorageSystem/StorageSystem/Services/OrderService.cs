using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StorageSystem.Models;

namespace StorageSystem.Services
{
    public static class OrderService
    {
        public static List<Order> Get()
        {
            using (var ctx = new StorageContext())
            {
                return [.. ctx.Orders];
            }
        }

        public static Order Get(int ID)
        {
            using (var ctx = new StorageContext())
            {
                return ctx.Orders.Where(o => o.ID == ID).Single();
            }
        }

        public static Order Create(int quantity, decimal discount, decimal price)
        {
            using (var ctx = new StorageContext())
            {
                var order = new Order { Quantity = quantity, Discount = discount, Price = price};
                ctx.Orders.Add(order);
                ctx.SaveChanges();
                return order;
            }
        }

        // Updates an order. Returns true if the database was updated.
        public static bool Update(Order order, int? quantity, decimal? discount, decimal? price)
        {
            using (var ctx = new StorageContext())
            {
                if (quantity.HasValue)
                    order.Quantity = quantity.Value;
                if (price.HasValue)
                    order.Price = price.Value;
                if (discount.HasValue)
                    order.Discount = discount.Value;

                ctx.Orders.Update(order);
                return 1 == ctx.SaveChanges();
            }
        }

        // Removes an order from the database. Returns true if successfully removed from database.
        public static bool Remove(Order order)
        {
            using (var ctx = new StorageContext())
            {
                ctx.Orders.Remove(order);
                return 1 == ctx.SaveChanges();
            }
        }
    }
}
