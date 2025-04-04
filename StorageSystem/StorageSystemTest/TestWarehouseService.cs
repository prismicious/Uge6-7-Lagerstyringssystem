using StorageSystem;
using StorageSystem.Models;
using StorageSystem.Services;

namespace StorageSystemTest
{
    [TestClass]
    public class TestWarehouseService
    {
        Warehouse wh1, wh2;
        List<Product> products;
        List<Customer> customers;

        [TestInitialize]
        public void Init()
        {
            using var ctx = new StorageContext();
            ctx.Database.EnsureCreated();

            // Create some test warehouses
            wh1 = WarehouseService.Create("test:location 1");
            wh2 = WarehouseService.Create("test:location 2");
            Assert.AreEqual(2, ctx.Warehouses.Count());

            // Create fake products and customers if needed
            if (ctx.Customers.Count() == 0 || ctx.Products.Count() == 0)
                FakerService.Generate();

            products = ctx.Products.ToList();
            customers = ctx.Customers.ToList();
        }

        [TestCleanup]
        public void Cleanup()
        {
            // Remove warehouses. Should also remove all product statuses
            WarehouseService.Remove(wh1);
            WarehouseService.Remove(wh2);
            
            using var ctx = new StorageContext();
            Assert.AreEqual(0, ctx.Warehouses.Count());
        }

        // Make sure that creating a warehouse also creates an internal customer
        [TestMethod]
        public void WarehouseCustomerExists()
        {
            using var ctx = new StorageContext();
            string name1 = $"warehouse:{wh1.ID}";
            string name2 = $"warehouse:{wh2.ID}";

            int numWarehouseCustomers = ctx.Customers.Where(c => c.Name == name1 || c.Name == name2).Count();
            Assert.AreEqual(2, numWarehouseCustomers);
        }

        [TestMethod]
        public void CreateProductStatuses()
        {
            // Create product statuses in each warehouse.
            // The quantity for each product is its ID.
            var products = ProductService.Get();
            foreach (var p in products)
            {
                WarehouseService.CreateProductStatus(wh1, p, p.ID);
                WarehouseService.CreateProductStatus(wh2, p, p.ID);
            }

            // Ensure the number of product statuses created is correct
            using var ctx = new StorageContext();
            Assert.AreEqual(2 * products.Count, ctx.ProductStatuses.Count());

            // Ensure the quantity for each product is correct
            foreach (var p in products)
            {
                int? quant1 = WarehouseService.GetProductStatus(wh1, p.ID)?.Quantity;
                int? quant2 = WarehouseService.GetProductStatus(wh2, p.ID)?.Quantity;
                Assert.AreEqual(p.ID, quant1);
                Assert.AreEqual(p.ID, quant2);
            }

            // Double the quantities and re-check
            foreach (var p in products)
            {
                WarehouseService.UpdateProductQuantity(wh1, p, p.ID);
                WarehouseService.UpdateProductQuantity(wh2, p, p.ID);

                int? quant1 = WarehouseService.GetProductStatus(wh1, p.ID)?.Quantity;
                int? quant2 = WarehouseService.GetProductStatus(wh2, p.ID)?.Quantity;
                Assert.AreEqual(2 * p.ID, quant1);
                Assert.AreEqual(2 * p.ID, quant2);
            }
        }

        [TestMethod]
        public void MoveBetweenWarehouses()
        {
            // TODO needs transaction service
        }
    }
}
