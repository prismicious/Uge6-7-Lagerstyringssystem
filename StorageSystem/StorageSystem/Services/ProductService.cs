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

        public static Product? Get(int ID)
        {
            using (var ctx = new StorageContext())
            {
                return ctx.Products.Where(p => p.ID == ID).SingleOrDefault();
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

        // Updates an existing product. Returns true if the database was updated.
        public static bool Update(Product p)
        {
            if (p.ID == 0 || Get(p.ID) == null)
                return false;

            using (var ctx = new StorageContext())
            {
                ctx.Products.Update(p);
                return 0 != ctx.SaveChanges();
            }
        }

        // Removes a product from the database. Returns true if successfully removed from database.
        public static bool Remove(Product p)
        {
            if (p.ID == 0 || Get(p.ID) == null)
                return false;

            using (var ctx = new StorageContext())
            {
                ctx.Products.Remove(p);
                return 0 != ctx.SaveChanges();
            }
        }

        // Removes a product from the database. Returns true if successfully removed from database.
        public static bool Remove(int ID)
        {
            if (ID == 0)
                return false;

            Product? p = Get(ID);
            if (p == null)
                return false;

            using (var ctx = new StorageContext())
            {
                ctx.Products.Remove(p);
                return 0 != ctx.SaveChanges();
            }
        }
    }
}
