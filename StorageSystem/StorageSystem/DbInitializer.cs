using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using StorageSystem.Models;

namespace StorageSystem
{
    public static class DbInitializer
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new StorageContext(
                serviceProvider.GetRequiredService<DbContextOptions<StorageContext>>()))
            {
                // Look for any products.
                if (context.Products.Any())
                {
                    return;   // DB has been seeded
                }

                var products = new Product[]
                {
                    new Product{Name="Product1", Price=10.0},
                    new Product{Name="Product2", Price=20.0},
                    new Product{Name="Product3", Price=30.0},
                };

                foreach (var p in products)
                {
                    context.Products.Add(p);
                }

                var warehouses = new Warehouse[]
                {
                    new Warehouse{Name="Warehouse1", Location="Location1"},
                    new Warehouse{Name="Warehouse2", Location="Location2"},
                };

                foreach (var w in warehouses)
                {
                    context.Warehouses.Add(w);
                }

                var orders = new Order[]
                {
                    new Order{OrderDate=DateTime.Now, ProductID=1, Quantity=5},
                    new Order{OrderDate=DateTime.Now, ProductID=2, Quantity=10},
                };

                foreach (var o in orders)
                {
                    context.Orders.Add(o);
                }

                context.SaveChanges();
            }
        }
    }
}
