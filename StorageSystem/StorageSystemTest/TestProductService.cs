using StorageSystem;
using StorageSystem.Models;
using StorageSystem.Services;

namespace StorageSystemTest
{
    [TestClass]
    public sealed class TestProductService
    {
        decimal price = 42.0m;
        string name = "Test product";
        string type = "Test type";

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
        public void Create()
        {
            int productCount = ProductService.Get().Count;

            // Test that the product is created correctly
            Product p = ProductService.Create(price, name, type);
            Assert.IsTrue(p != null);
            Assert.IsTrue(p.ID > 0);
            Assert.AreEqual(price, p.Price);
            Assert.AreEqual(name, p.Name);
            Assert.AreEqual(type, p.Type);

            // Ensure the product count increased
            Assert.IsTrue(ProductService.Get().Count > productCount);
        }

        [TestMethod]
        public void Read()
        {
            Create();
            Product p = ProductService.Get(1);
            Assert.IsTrue(p != null);
            Assert.AreEqual(1, p.ID);
            Assert.AreEqual(price, p.Price);
            Assert.AreEqual(name, p.Name);
            Assert.AreEqual(type, p.Type);
        }

        [TestMethod]
        public void Update()
        {
            Product p = ProductService.Create(123.45m, "First name", "Type");

            var newName = "Updated name";
            bool updated = ProductService.Update(p, Name: newName);
            Assert.IsTrue(updated);

            p = ProductService.Get(p.ID);
            Assert.AreEqual(newName, p.Name);
        }

        [TestMethod]
        public void Delete()
        {
            Create();
            var p = ProductService.Get().First();
            Assert.IsTrue(ProductService.Remove(p));
        }
    }
}
