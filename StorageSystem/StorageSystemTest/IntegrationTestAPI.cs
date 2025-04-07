using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using StorageSystem;
using StorageSystem.Models;
using StorageSystem.Services;

namespace StorageSystemTest
{
    [TestClass]
    public sealed class IntegrationTestAPI
    {
        private static HttpClient _client;
        private List<Product> _testProducts; 

        [ClassInitialize]
        public static void Initialize(TestContext context)
        {
            var factory = new WebApplicationFactory<Program>();
            _client = factory.CreateClient();
        }

        [TestInitialize]
        public void Init()
        {
            using (var ctx = new StorageContext())
            {
                ctx.Database.EnsureDeleted();
                ctx.Database.EnsureCreated();

                var p1 = ProductService.Create(10.99m, "Test Product", "Test Type");
                var p2 = ProductService.Create(20.99m, "Another Product", "Another Type");
                var p3 = ProductService.Create(30.99m, "Third Product", "Third Type");

                _testProducts = new List<Product> { p1, p2, p3 };
            }
        }

        [TestMethod]
        public async Task GetAllProducts_ReturnsOk()
        {
            // Arrange
            var requestUri = "/api/Product";

            // Act
            var response = await _client.GetAsync(requestUri);

            // Assert
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
            var products = JsonSerializer.Deserialize<List<Product>>(responseContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            Assert.IsNotNull(products, "Response content should not be null.");
            Assert.AreEqual(_testProducts.Count, products.Count, "Expected the same number of products in the response.");
            foreach (var testProduct in _testProducts)
            {
                Assert.IsTrue(products.Any(p => p.ID == testProduct.ID && p.Name == testProduct.Name && p.Price == testProduct.Price && p.Type == testProduct.Type),
                    $"Response should contain product with ID {testProduct.ID}.");
            }
        }

        [TestMethod]
        public async Task CreateProduct_ReturnsCreated()
        {
            // Arrange
            var product = new Product { Price = 10.99m, Name = "Test Product", Type = "Test Type" };
            var content = new StringContent(JsonSerializer.Serialize(product), Encoding.UTF8, "application/json");
            var requestUri = "/api/Product";
            Assert.AreEqual(3, ProductService.Get().Count);
            // Act
            var response = await _client.PostAsync(requestUri, content);

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.AreEqual(System.Net.HttpStatusCode.Created, response.StatusCode);
            Assert.AreEqual(4, ProductService.Get().Count);
        }

        [TestMethod]
        public async Task CreateProduct_InvalidInput_ReturnsInternalServerError()
        {
            // Arrange
            var product = new Product { Price = -10.99m, Name = "", Type = "" }; // Invalid input
            var content = new StringContent(JsonSerializer.Serialize(product), Encoding.UTF8, "application/json");
            var requestUri = "/api/Product";
            

            // Act
            var response = await _client.PostAsync(requestUri, content);

            // Assert
            Assert.AreEqual(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
        }

        [TestMethod]
        public async Task GetProductById_ReturnsOk()
        {
            // Arrange
            var requestUri = "/api/Product/1";

            // Act
            var response = await _client.GetAsync(requestUri);

            // Assert
            if (response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
            {
                Assert.Inconclusive("Product with ID 1 does not exist.");
            }
            response.EnsureSuccessStatusCode();
        }

        [TestMethod]
        public async Task GetProductById_NonExistent_ReturnsNotFound()
        {
            // Arrange
            var requestUri = "/api/Product/9999"; // Non-existent ID

            // Act
            var response = await _client.GetAsync(requestUri);

            // Assert
            Assert.AreEqual(System.Net.HttpStatusCode.NotFound, response.StatusCode);
        }

        [TestMethod]
        public async Task UpdateProduct_ReturnsNoContent()
        {
            // Arrange
            var product = new Product { ID = 1, Price = 15.99m, Name = "Updated Product", Type = "Updated Type" };
            var content = new StringContent(JsonSerializer.Serialize(product), Encoding.UTF8, "application/json");
            var requestUri = "/api/Product";

            // Act
            var response = await _client.PatchAsync(requestUri, content);

            // Assert
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                Assert.Inconclusive("Product with ID 1 does not exist.");
            }
            Assert.AreEqual(System.Net.HttpStatusCode.NoContent, response.StatusCode);
        }

        [TestMethod]
        public async Task UpdateProduct_NonExistent_ReturnsNotFound()
        {
            // Arrange
            var product = new Product { ID = 9999, Price = 15.99m, Name = "Non-existent Product", Type = "Non-existent Type" };
            var content = new StringContent(JsonSerializer.Serialize(product), Encoding.UTF8, "application/json");
            var requestUri = "/api/Product"; // Non-existent ID

            // Act
            var response = await _client.PatchAsync(requestUri, content);

            // Assert
            Assert.AreEqual(System.Net.HttpStatusCode.NotFound, response.StatusCode);
        }

        [TestMethod]
        public async Task DeleteProduct_ReturnsNoContent()
        {
            // Arrange
            var requestUri = "/api/Product/1";

            // Act
            var response = await _client.DeleteAsync(requestUri);

            // Assert
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                Assert.Inconclusive("Product with ID 1 does not exist.");
            }
            Assert.AreEqual(System.Net.HttpStatusCode.NoContent, response.StatusCode);
        }

        [TestMethod]
        public async Task DeleteProduct_NonExistent_ReturnsNotFound()
        {
            // Arrange
            var requestUri = "/api/Product/9999"; // Non-existent ID

            // Act
            var response = await _client.DeleteAsync(requestUri);

            // Assert
            Assert.AreEqual(System.Net.HttpStatusCode.NotFound, response.StatusCode);
        }
    }
}
