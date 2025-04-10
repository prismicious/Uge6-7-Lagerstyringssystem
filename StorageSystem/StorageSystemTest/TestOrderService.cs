using StorageSystem;
using StorageSystem.Models;
using StorageSystem.Services;

namespace StorageSystemTest
{
    [TestClass]
    public sealed class TestOrderService
    {
        int quantity = 12;
        decimal discount = 1.0m;
        decimal price = 42.0m;
        int lastCreatedOrderID = 0;

        Customer testCustomer = new Customer { Name = "Test", Email = "", Address = "" };
        Product testProduct = new Product { Name = "Order test product", Price = 1.0m, Type = "Test product" };
        OrderList testOrderList = new();

        [TestInitialize]
        public void Init()
        {
            using (var ctx = new StorageContext())
            {
                ctx.Database.EnsureDeleted();
                ctx.Database.EnsureCreated();
                Assert.AreEqual(0, ctx.Orders.Count());

                ctx.Customers.Add(testCustomer);
                ctx.SaveChanges();

                ctx.Products.Add(testProduct);
                ctx.SaveChanges();

                testOrderList.CustomerID = testCustomer.ID;
                testOrderList.Customer = testCustomer;
                ctx.OrderLists.Add(testOrderList);
                ctx.SaveChanges();
            }
        }

        [TestMethod]
        public void Create()
        {
            int count = OrderService.Get().Count;

            // Test that the product is created correctly
            Order order = OrderService.Create(testOrderList, testProduct, quantity, discount, price);
            Assert.IsTrue(order != null);
            Assert.IsTrue(order.ID > 0);
            Assert.AreEqual(quantity, order.Quantity);
            Assert.AreEqual(discount, order.Discount);
            Assert.AreEqual(price, order.Price);

            // Ensure the order count increased
            Assert.IsTrue(OrderService.Get().Count == 1 + count);

            // Save the order ID for later use
            lastCreatedOrderID = order.ID;
        }

        [TestMethod]
        public void Read()
        {
            Create();

            Order order = OrderService.Get(lastCreatedOrderID);

            Assert.IsTrue(order != null);
            Assert.AreEqual(lastCreatedOrderID, order.ID);
            Assert.AreEqual(quantity, order.Quantity);
            Assert.AreEqual(discount, order.Discount);
            Assert.AreEqual(price, order.Price);
        }

        [TestMethod]
        public void Update()
        {
            // Create a new order
            Order order = OrderService.Create(testOrderList, testProduct, 3, 1.22m, 123.45m);

            // Update the order's price
            const decimal newPrice = 99.99m;
            order.Price = newPrice;
            bool updated = OrderService.Update(order);
            Assert.IsTrue(updated);

            // Re-load the order from the db and verify new price
            order = OrderService.Get(order.ID);
            Assert.AreEqual(newPrice, order.Price);
        }

        [TestMethod]
        public void Delete()
        {
            // Make a new order
            Create();

            // Get the order from the db
            var order = OrderService.Get(lastCreatedOrderID);
            Assert.IsNotNull(order);

            // Delete it
            Assert.IsTrue(OrderService.Remove(order));
        }
    }
}
