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
        private StorageContext CreateInMemoryContext(string dbName)
        {
            var options = new DbContextOptionsBuilder<StorageContext>()
                .UseInMemoryDatabase(databaseName: dbName) // Unique name for each test
                .Options;

            return new StorageContext(options);
        }

        [TestMethod]
        public void TestInsertData()
        {
            // Arrange
            int productCount = 100;
            int customerCount = 50;
            string dbName = "TestDatabase";

            using (var ctx = CreateInMemoryContext(dbName))
            {
                // Act
                FakerService.GenerateAndPopulate(ctx, productCount, customerCount);
                ctx.SaveChanges(); // Ensure data is saved

                // Assert
                Assert.AreEqual(productCount, ctx.Products.Count(), "Products count mismatch");
                Assert.AreEqual(customerCount, ctx.Customers.Count(), "Customers count mismatch");
            }
        }
    }
}
