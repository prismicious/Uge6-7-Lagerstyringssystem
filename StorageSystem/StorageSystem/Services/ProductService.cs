using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StorageSystem;
using StorageSystem.Models;

namespace StorageSystem.Services
{
    internal class ProductService
    {
        static List<Product> GetProducts()
        {
            using (var ctx = new StorageContext())
            {
                return [.. ctx.Products];
            }
        }

        static void AddProduct()
        {
            using (var ctx = new StorageContext())
            {
                ctx.Products.Add(new Product { Price = 1.0m, Name = "test", Type = "meh" });
                ctx.SaveChanges();
            }
        }
    }
}
