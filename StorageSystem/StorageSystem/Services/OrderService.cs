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
                var order = ctx.Orders.Where(o => o.ID == ID).SingleOrDefault();
                if (order == null)
                    return null;
                
                return order;
            }
        }

        public static Order Create(OrderList list, Product p, int quantity, decimal discount, decimal price)
        {
            using (var ctx = new StorageContext())
            {
                var order = new Order { Quantity = quantity, Discount = discount, Price = price, ProductID = p.ID, OrderListID = list.ID };
                ctx.Orders.Add(order);
                ctx.OrderLists.Update(list);
                ctx.SaveChanges();
                return order;
            }
        }

        // Updates an order. Returns true if the database was updated.
        public static bool Update(Order order)
        {
            using (var ctx = new StorageContext())
            {
                ctx.Orders.Update(order);
                return 0 != ctx.SaveChanges();
            }
        }

        // Removes an order from the database. Returns true if successfully removed from database.
        public static bool Remove(Order order)
        {
            using (var ctx = new StorageContext())
            {
                ctx.Orders.Remove(order);
                return 0 != ctx.SaveChanges();
            }
        }
    }
}
