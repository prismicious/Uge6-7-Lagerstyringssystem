using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StorageSystem;
using StorageSystem.Services;
using System.Linq;

namespace StorageSystemTest
{
    [TestClass]
    public sealed class FakerServiceTest
    {
        [TestInitialize]
        public void Init()
        {
            using (var ctx = new StorageContext())
            {
                ctx.Database.EnsureDeleted();
                ctx.Database.EnsureCreated();
                Assert.AreEqual(0, ctx.Products.Count());
            }
        }
        [TestMethod]
        public void TestInsertData()
        {
            // Arrange
            int productCount = 100;
            int customerCount = 50;
            int productStatusCount = productCount;

            using (var context = new StorageContext())
            {
                // Act
                context.Database.EnsureCreated(); // Ensure database schema is created
                FakerService.GenerateAndPopulate(productCount, customerCount);
                context.SaveChanges(); // Ensure data is saved

                // Assert
                Assert.AreEqual(productCount, context.Products.Count(), "Products count mismatch");
                Assert.AreEqual(customerCount, context.Customers.Count(), "Customers count mismatch");
                Assert.AreEqual(productStatusCount, context.ProductStatuses.Count(), "Product statuses count mismatch");
            }
        }
    }
}
