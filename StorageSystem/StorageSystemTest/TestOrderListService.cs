using StorageSystem;
using StorageSystem.Models;
using StorageSystem.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StorageSystemTest
{
    // Tests for the OrderList service class.
    // Test creation of the order list, and adding/removing orders.
    [TestClass]
    public sealed class TestOrderListService
    {
        Customer testCustomer = new Customer { Name = "OrderList customer test", Email = "", Address = "" };
        Product testProduct = new Product { Name = "OrderList test product", Price = 1.0m, Type = "Test" };

        [TestInitialize]
        public void Init()
        {
            using (var ctx = new StorageContext())
            {
                ctx.Database.EnsureDeleted();
                ctx.Database.EnsureCreated();

                // Create a test customer
                ctx.Customers.Add(testCustomer);
                ctx.SaveChanges();
                Assert.IsTrue(testCustomer.ID > 0, "Test customer was not created.");

                // Create a test product
                ctx.Products.Add(testProduct);
                ctx.SaveChanges();
                Assert.IsTrue(testProduct.ID > 0, "Test product was not created.");
            }
        }

        [TestCleanup]
        public void Cleanup()
        {
            using (var ctx = new StorageContext())
            {
                // Delete the test customer and product
                var customer = ctx.Customers.FirstOrDefault(c => c.Name == testCustomer.Name);
                if (customer != null)
                {
                    ctx.Customers.Remove(customer);
                    ctx.SaveChanges();
                }
                var product = ctx.Products.FirstOrDefault(p => p.Name == testProduct.Name);
                if (product != null)
                {
                    ctx.Products.Remove(product);
                    ctx.SaveChanges();
                }
            }
        }

        [TestMethod]
        public void Create()
        {
            using (var ctx = new StorageContext())
            {
                // Create an order list for the test customer
                var orderList = OrderListService.Create(testCustomer);
                Assert.IsNotNull(orderList);
                Assert.IsTrue(orderList.ID > 0);
                Assert.AreEqual(testCustomer.ID, orderList.CustomerID);

                // Ensure the order list is saved in the database
                var savedOrderList = OrderListService.Get(orderList.ID);
                Assert.IsNotNull(savedOrderList);
                Assert.AreEqual(orderList.CustomerID, savedOrderList.CustomerID);
            }
        }

        [TestMethod]
        public void AddOrder()
        {
            using (var ctx = new StorageContext())
            {
                // Create an order list for the test customer
                var orderList = OrderListService.Create(testCustomer);
                Assert.IsNotNull(orderList);

                // Create a test order
                var order = OrderService.Create(orderList, testProduct, 1, 1, 1);
                orderList = OrderListService.AddOrder(orderList, order);

                // Ensure the order is added to the order list
                var fetchedOrderList = OrderListService.Get(orderList.ID);
                Assert.AreEqual(fetchedOrderList.ID, order.OrderListID);

                var fetchedOrder = fetchedOrderList.Orders.Where(o => o.ID == order.ID).Single();
                Assert.AreEqual(fetchedOrder.OrderListID, order.OrderListID);
            }
        }

        [TestMethod]
        public void RemoveOrder()
        {
            using (var ctx = new StorageContext())
            {
                // Create an order list for the test customer
                var orderList = OrderListService.Create(testCustomer);
                Assert.IsNotNull(orderList);

                // Create a test order
                var order = OrderService.Create(orderList, testProduct, 1, 1, 1);
                orderList = OrderListService.AddOrder(orderList, order);

                // Ensure the order is added to the order list
                var fetchedOrderList = OrderListService.Get(orderList.ID);
                Assert.AreEqual(fetchedOrderList.ID, order.OrderListID);

                // Remove the order from the order list
                orderList = OrderListService.RemoveOrder(orderList, order);

                // Ensure the order is removed from the order list
                fetchedOrderList = OrderListService.Get(orderList.ID);
                Assert.IsFalse(fetchedOrderList.Orders.Any(o => o.ID == order.ID));
            }
        }
    }
}
