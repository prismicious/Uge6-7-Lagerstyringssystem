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
        int createTestID = 0;

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

            createTestID = order.ID;
        }

        [TestMethod]
        public void Read()
        {
            Create();
            Order order = OrderService.Get(createTestID);
            Assert.IsTrue(order != null);
            Assert.AreEqual(createTestID, order.ID);
            Assert.AreEqual(quantity, order.Quantity);
            Assert.AreEqual(discount, order.Discount);
            Assert.AreEqual(price, order.Price);
        }

        [TestMethod]
        public void Update()
        {
            Order order = OrderService.Create(testOrderList, testProduct, 3, 1.22m, 123.45m);

            var newPrice = 100.00m;
            bool updated = OrderService.Update(order, price: newPrice);
            Assert.IsTrue(updated);

            // Re-load the order from the db
            order = OrderService.Get(order.ID);
            Assert.AreEqual(newPrice, order.Price);
        }

        [TestMethod]
        public void Delete()
        {
            Create();
            var order = OrderService.Get().First();
            Assert.IsTrue(OrderService.Remove(order));
        }
    }
}
