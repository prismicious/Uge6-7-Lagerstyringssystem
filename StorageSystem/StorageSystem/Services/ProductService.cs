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
        static List<ProductA> GetProducts()
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
                ctx.Products.Add(new ProductA { Price = 1.0m, Name = "test", Type = "meh" });
                ctx.SaveChanges();
            }
        }
    }
}
