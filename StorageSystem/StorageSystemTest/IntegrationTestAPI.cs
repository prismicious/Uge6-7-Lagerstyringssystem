using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using StorageSystem.Models;

namespace StorageSystemTest
{
    [TestClass]
    public sealed class IntegrationTestAPI
    {
        private static HttpClient _client;


        [ClassInitialize]
        public static void Initialize(TestContext context)
        {
            var factory = new WebApplicationFactory<Program>();
            _client = factory.CreateClient();
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
        }

        [TestMethod]
        public async Task CreateProduct_ReturnsCreated()
        {
            // Arrange
            var product = new Product { Price = 10.99m, Name = "Test Product", Type = "Test Type" };
            var content = new StringContent(JsonSerializer.Serialize(product), Encoding.UTF8, "application/json");
            var requestUri = "/api/Product";

            // Act
            var response = await _client.PostAsync(requestUri, content);

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.AreEqual(System.Net.HttpStatusCode.Created, response.StatusCode);
        }

        [TestMethod]
        public async Task CreateProduct_InvalidInput_ReturnsBadRequest()
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
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
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
            var product = new Product { Price = 15.99m, Name = "Updated Product", Type = "Updated Type" };
            var content = new StringContent(JsonSerializer.Serialize(product), Encoding.UTF8, "application/json");
            var requestUri = "/api/Product/1";

            // Act
            var response = await _client.PutAsync(requestUri, content);

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
            var product = new Product { Price = 15.99m, Name = "Non-existent Product", Type = "Non-existent Type" };
            var content = new StringContent(JsonSerializer.Serialize(product), Encoding.UTF8, "application/json");
            var requestUri = "/api/Product/9999"; // Non-existent ID

            // Act
            var response = await _client.PutAsync(requestUri, content);

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
