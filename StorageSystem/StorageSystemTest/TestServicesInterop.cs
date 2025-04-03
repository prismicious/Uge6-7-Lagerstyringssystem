using StorageSystem;
using StorageSystem.Models;
using StorageSystem.Services;

namespace StorageSystemTest
{
    // Tests for checking functionallity across services.
    [TestClass]
    public class TestServicesInterop
    {
        Customer testCustomer = new Customer { Name = "ServicesInterop customer test", Email = "", Address = "" };
        Product testProduct = new Product { Name = "ServicesInterop test product", Price = 1.0m, Type = "Test" };

        [TestInitialize]
        public void Init()
        {
            using (var ctx = new StorageContext())
            {
                ctx.Database.EnsureDeleted();
                ctx.Database.EnsureCreated();

                // Create the test customer
                ctx.Customers.Add(testCustomer);
                ctx.SaveChanges();
                Assert.IsTrue(testCustomer.ID > 0, "Test customer was not created.");

                // Create the test product
                ctx.Products.Add(testProduct);
                ctx.SaveChanges();
                Assert.IsTrue(testProduct.ID > 0, "Test product was not created.");

                // Create a test warehouse
                var wh = new Warehouse { Location = "Test Address" };
                ctx.Warehouses.Add(wh);
                ctx.SaveChanges();

                // Add stock to the warehouse
                ctx.ProductStatuses.Add(new ProductStatus { ProductID = testProduct.ID, Warehouse = wh, Quantity = 20, Reserved = 0 });
                ctx.SaveChanges();
            }
        }

        // Test that creating orders reserves product
        [TestMethod]
        public void PlacingOrderReservesProduct()
        {
            // Inital count of products in stock. Should be 20.
            int countStock = ProductService.CountStock(testProduct);
            Assert.AreEqual(20, countStock);

            // Initial count of reserved products. Should be zero.
            int countReserved = ProductService.CountReserved(testProduct);
            Assert.AreEqual(0, countReserved);

            // Create orders for 7 units
            var orderList = OrderListService.Create(testCustomer);
            var order1 = OrderService.Create(orderList, testProduct, 3, 1, testProduct.Price);
            var order2 = OrderService.Create(orderList, testProduct, 4, 1, testProduct.Price);

            // Check the reserve
            countReserved = ProductService.CountReserved(testProduct);
            Assert.AreEqual(7, countReserved);

            // Delete the order list. This will also delete all the associated orders.
            bool success = OrderListService.Remove(orderList);
            Assert.IsTrue(success);
            Assert.IsTrue(OrderService.Get().Count == 0);

            // Check the reserve again
            countReserved = ProductService.CountReserved(testProduct);
            Assert.AreEqual(0, countReserved);
        }
    }
}
