﻿using StorageSystem.Models;

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
        public static bool Update(Product p)
        {
            using (var ctx = new StorageContext())
            {
                ctx.Products.Update(p);
                return 1 == ctx.SaveChanges();
            }
        }

        // Removes a product from the database. Returns true if successfully removed from database.
        public static bool Remove(Product p)
        {
            using (var ctx = new StorageContext()) { 
                ctx.Products.Remove(p);
                return 1 == ctx.SaveChanges();
            }
        }
    }
}
