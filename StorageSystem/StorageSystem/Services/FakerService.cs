using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bogus;
using StorageSystem.Models;

namespace StorageSystem.Services
{
    public class FakerService
    {
        public static void GenerateAndPopulate(int productCount, int customerCount)
        {
            LogService.Log("Starting data generation...");

            // Helper method to create a Faker<Product> for a specific category
            Faker<Product> CreateProductFaker(string category, string[] names, decimal minPrice, decimal maxPrice)
            {
                return new Faker<Product>()
                    .RuleFor(p => p.Name, f => f.PickRandom(names))
                    .RuleFor(p => p.Price, f => f.Random.Decimal(minPrice, maxPrice))
                    .RuleFor(p => p.Type, _ => category);
            }

            // Define product categories and their specific rules
            var categories = new[]
            {
                new { Category = "Electronics", Names = new[] { "Smartphone", "Laptop", "Smart TV", "Headphones", "Camera" }, MinPrice = 100m, MaxPrice = 2000m },
                new { Category = "Software", Names = new[] { "Antivirus Software", "Photo Editor", "Accounting Software", "Video Editor", "IDE" }, MinPrice = 50m, MaxPrice = 500m },
                new { Category = "Hardware", Names = new[] { "Graphics Card", "Processor", "Motherboard", "RAM", "Power Supply", "HDD", "SSD" }, MinPrice = 20m, MaxPrice = 1000m },
                new { Category = "Utilities", Names = new[] { "Power Adapter", "Extension Cord", "Battery Pack", "Surge Protector", "Tool Kit" }, MinPrice = 10m, MaxPrice = 200m }
            };

            // Generate products
            var products = new List<Product>();
            foreach (var category in categories)
            {
                // Generate products for the current category
                var productFaker = CreateProductFaker(category.Category, category.Names, category.MinPrice, category.MaxPrice);
                products.AddRange(productFaker.Generate(productCount / categories.Length));
            }

            // Generate customers
            var customerFaker = new Faker<Customer>()
                .RuleFor(c => c.Name, f => f.Name.FullName())
                .RuleFor(c => c.Email, f => f.Internet.Email())
                .RuleFor(c => c.Address, f => f.Address.FullAddress())
                .RuleFor(c => c.Type, f => f.Random.Number(0, 2)); // 0: Customer, 1: Business, 2: Warehouse
            var customers = customerFaker.Generate(customerCount);

            // Generate warehouses
            var warehouses = new Faker<Warehouse>()
                .RuleFor(w => w.Location, f => f.Address.City())
                .RuleFor(w => w.ProductStatuses, _ => new List<ProductStatus>())
                .RuleFor(w => w.Transactions, _ => new List<Transaction>())
                .Generate(5); // Create 5 warehouses

            // Generate product statuses
            var productStatuses = new Faker<ProductStatus>()
                .RuleFor(ps => ps.ProductID, f => f.Random.Number(1, productCount))
                .RuleFor(ps => ps.Quantity, f => f.Random.Number(1, 100))
                .RuleFor(ps => ps.Reserved, f => f.PickRandom(0, 1))
                .Generate(productCount); // Generate product statuses for all products

                

            // Assign product statuses to warehouses randomly
            var random = new Random();
            foreach (var productStatus in productStatuses)
            {
                var randomWarehouse = warehouses[random.Next(warehouses.Count)];
                productStatus.WarehouseID = randomWarehouse.ID; // Assign a random warehouse ID
                randomWarehouse.ProductStatuses.Add(productStatus); // Add to the warehouse's ProductStatuses list
            }

            using (var ctx = new StorageContext())
            {
                // Add warehouses, products, and customers to the context
                ctx.Warehouses.AddRange(warehouses);
                ctx.Products.AddRange(products);
                ctx.Customers.AddRange(customers);
                ctx.ProductStatuses.AddRange(productStatuses);
                ctx.SaveChanges();
                LogService.Log($"Inserted {products.Count} products and {customers.Count} customers into the database.");
            }
        }
        public static void Generate()
        {
            LogService.Log("Initializing database population...");
            int productsToGenerate = 100;
            int customersToGenerate = 20;
            using (var context = new StorageContext())
            {
                context.Database.EnsureCreated(); // Ensure the database is created
                GenerateAndPopulate(productsToGenerate, customersToGenerate); // Always populate the database

            }
            LogService.Log("Database population completed.");
        }
    }
}
