using StorageSystem.Models;

namespace StorageSystem.Services
{
    public static class ProductService
    {
        static List<Product> Get()
        {
            using (var ctx = new StorageContext())
            {
                return [.. ctx.Products];
            }
        }

        // Creates a new product. Returns the new product ID.
        static int Create(decimal Price, string Name, string Type)
        {
            using (var ctx = new StorageContext())
            {
                var p = new Product { Price = Price, Name = Name, Type = Type };
                ctx.Products.Add(p);
                ctx.SaveChanges();
                return p.ID;
            }
        }

        // Updates a product. Returns true if the database was updated.
        static bool Update(Product p, decimal? Price = null, string? Name = null, string? Type = null)
        {
            using (var ctx = new StorageContext())
            {
                if (Price.HasValue)
                    p.Price = Price.Value;
                if (Name != null)
                    p.Name = Name;
                if (Type != null)
                    p.Type = Type;
                return 1 == ctx.SaveChanges();
            }
        }

        // Removes a product from the database. Returns true if successfully removed from database.
        static bool Remove(Product p)
        {
            using (var ctx = new StorageContext()) { 
                ctx.Products.Remove(p);
                return 1 == ctx.SaveChanges();
            }
        }
    }
}
