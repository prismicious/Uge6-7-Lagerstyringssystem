using StorageSystem;
using StorageSystem.Models;
using StorageSystem.Services;

namespace StorageSystemTest
{
    [TestClass]
    public class TestProductStatusService
    {
        List<Product> products;
        List<Customer> customers;
        List<ProductStatus> statuses;

        [TestInitialize]
        public void Init()
        {
            using var ctx = new StorageContext();
            ctx.Database.EnsureDeleted();
            ctx.Database.EnsureCreated();

            // Create fake products and customers
            FakerService.Generate();

            products = ctx.Products.ToList();
            customers = ctx.Customers.ToList();
            statuses = ctx.ProductStatuses.ToList();

            Assert.AreEqual(products.Count, statuses.Count);
        }

        [TestMethod]
        public void UpdateProductStatusesQuantity()
        {
            int quantity = ProductStatusService.GetTotalQuantity();
            int expectedIncrease = quantity + products.Count;

            // Update each product quantity by 1.
            foreach (var p in products)
                ProductStatusService.Update(p, 1);

            // Ensure the updated product quantity is correct
            Assert.AreEqual(expectedIncrease, ProductStatusService.GetTotalQuantity());
        }

        [TestMethod]
        public void ReservingProducts()
        {
            // FakerService reserves around 50 products
            int initialReserved = ProductStatusService.GetTotalReserved();

            // Reserve 1 product for each product in the list
            foreach (var p in products)
                ProductStatusService.Reserve(p.ID, 1);

            int expectedReserved = initialReserved + products.Count;
            Assert.AreEqual(expectedReserved, ProductStatusService.GetTotalReserved());

            // Undo the reservations
            foreach (var p in products)
                ProductStatusService.UndoReserve(p.ID, 1);
            Assert.AreEqual(initialReserved, ProductStatusService.GetTotalReserved());
        }

        [TestMethod]
        public void GetAllProductStatuses()
        {
            List<ProductStatus>? result = ProductStatusService.Get();
            Assert.IsNotNull(result);
            Assert.AreEqual(products.Count, result.Count);
        }
    }
}
