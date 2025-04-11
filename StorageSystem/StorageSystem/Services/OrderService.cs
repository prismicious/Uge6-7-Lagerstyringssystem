using StorageSystem.Models;
using StorageSystem.Strategies;

namespace StorageSystem.Services
{
    public static class OrderService
    {
        public static List<Order> Get()
        {
            using var ctx = new StorageContext();
            return [.. ctx.Orders];
        }

        public static Order? Get(int ID)
        {
            using var ctx = new StorageContext();
            return ctx.Orders.Where(o => o.ID == ID).SingleOrDefault();
        }

        public static Order Create(OrderList list, Product p, int quantity, IDiscountStrategy? discountStrategy = null)
        {
            if (p.ID == 0)
                throw new ArgumentException("Invalid product ID");

            ProductStatusService.Reserve(p.ID, quantity);

            // Calculate a potential discount
            decimal discount = (null != discountStrategy) ? discountStrategy.Calculate(p, quantity) : 0.0m;

            using var ctx = new StorageContext();
            var order = new Order { Quantity = quantity, Price = p.Price, Discount = discount, ProductID = p.ID, OrderListID = list.ID };
            ctx.Orders.Add(order);
            ctx.OrderLists.Update(list);
            ctx.SaveChanges();
            return order;
        }

        // Updates an order. Returns true if the database was updated.
        public static bool Update(Order order)
        {
            using var ctx = new StorageContext();
            ctx.Orders.Update(order);
            return 0 != ctx.SaveChanges();
        }

        // Removes an order from the database. Returns true if successfully removed from database.
        public static bool Remove(Order order)
        {
            using var ctx = new StorageContext();
            ctx.Orders.Remove(order);
            if (0 == ctx.SaveChanges())
                return false;

            ProductStatusService.UndoReserve(order.ProductID, order.Quantity);
            return true;
        }

        public static decimal CalculateDiscountedPrice(Order order)
        {
            decimal price = order.Price * order.Quantity;
            decimal discount = order.Discount;
            return price - (price * discount);
        }
    }
}
