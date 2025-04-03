using Microsoft.EntityFrameworkCore;
using StorageSystem.Models;

namespace StorageSystem.Services
{
    public static class ProductService
    {
        public static List<Product> Get()
        {
            using (var ctx = new StorageContext())
            {
                return [.. ctx.Products];
            }
        }

        public static Product Get(int ID)
        {
            using (var ctx = new StorageContext())
            {
                return ctx.Products.Where(p => p.ID == ID).Single();
            }
        }

        public static Product Create(decimal Price, string Name, string Type)
        {
            using (var ctx = new StorageContext())
            {
                var p = new Product { Price = Price, Name = Name, Type = Type };
                ctx.Products.Add(p);
                ctx.SaveChanges();
                return p;
            }
        }

        // Updates a product. Returns true if the database was updated.
        public static bool Update(Product p, decimal? Price = null, string? Name = null, string? Type = null)
        {
            using (var ctx = new StorageContext())
            {
                if (Price.HasValue)
                    p.Price = Price.Value;
                if (Name != null)
                    p.Name = Name;
                if (Type != null)
                    p.Type = Type;

                ctx.Products.Update(p);
                return 1 == ctx.SaveChanges();
            }
        }

        // Removes a product from the database. Returns true if successfully removed from database.
        public static bool Remove(Product p)
        {
            using (var ctx = new StorageContext())
            {
                ctx.Products.Remove(p);
                return 1 == ctx.SaveChanges();
            }
        }

        // Determine the available stock across warehouses
        public static int CountStock(Product p)
        {
            if (p.ID < 1)
                return 0;
            using (var ctx = new StorageContext())
            {
                return ctx.Warehouses
                    .Include(w => w.ProductStatuses)
                    .Where(w => w.ProductStatuses.Any(p2 => p2.ProductID == p.ID))
                    .Select(w => w.ProductStatuses.Where(p2 => p2.ProductID == p.ID).Sum(p2 => p2.Quantity))
                    .FirstOrDefault();
            }
        }

        // Determine the number of reserved products
        public static int CountReserved(Product p)
        {
            if (p.ID < 1)
                return 0;

            using (var ctx = new StorageContext())
            {
                return ctx.Orders
                    .Include(o => o.OrderList)
                    .Where(o => o.ProductID == p.ID)
                    .Where(o => o.OrderList.Transaction == null)
                    .Select(o => o.Quantity)
                    .Sum();
            }
        }
    }
}
