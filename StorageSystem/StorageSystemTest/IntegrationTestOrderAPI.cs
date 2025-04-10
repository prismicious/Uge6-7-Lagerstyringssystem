using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using StorageSystem;
using StorageSystem.DTOs;
using StorageSystem.Models;
using StorageSystem.Services;


namespace StorageSystemTest;

[TestClass]
public class IntegrationTestOrderAPI
{
    private static HttpClient _client;
    private List<Order> _testOrders;

    [ClassInitialize]
    public static void InitializeClass(TestContext context)
    {
        var factory = new WebApplicationFactory<Program>();
        _client = factory.CreateClient();
    }

    [TestInitialize]
    public void InitialiseTests()
    {
        using (var ctx = new StorageContext())
        {
            ctx.Database.EnsureDeleted();
            ctx.Database.EnsureCreated();
            Customer customer = new Customer() { ID = 1, Name = "John", Email = "JohnMadden@handegg.com", Address = "USA" };
            ctx.Customers.Add(customer);
            ctx.SaveChanges();
            Product testProduct = ProductService.Create(1.0m, "TestProduct", "BestInTestTest");
            OrderList orderlist = OrderListService.Create(customer);
            Order order1 = OrderService.Create(orderlist, testProduct, 50, 0, 100);
            Order order2 = OrderService.Create(orderlist, testProduct, 500, 0, 1000);
            Order order3 = OrderService.Create(orderlist, testProduct, 5000, 0, 2000);

            _testOrders = new List<Order> { order1, order2, order3 };
        }
    }


    [TestMethod]
    public async Task GetAllOrders()
    {
        // Arrange 
        string requestUri = "/api/Order";

        //Act 
        var response = await _client.GetAsync(requestUri);
        var responseContent = await response.Content.ReadAsStringAsync();
        var orders = JsonSerializer.Deserialize<List<Order>>(responseContent, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        //Assert
        response.EnsureSuccessStatusCode();

        Assert.IsNotNull(orders, "Response content should not be null.");
        Assert.AreEqual(_testOrders.Count, orders.Count, "Expected the same number of products in the response.");
    }

    [TestMethod]
    public async Task GetOrderFromID()
    {
        // Arrange 
        string requestUri = "/api/Order/1";

        //Act 
        var response = await _client.GetAsync(requestUri);
        var responseContent = await response.Content.ReadAsStringAsync();
        var order = JsonSerializer.Deserialize<Order>(responseContent, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
        //Assert
        Assert.IsNotNull(order);
        Assert.AreEqual(50, order.Quantity);
    }
    // I dont know if this is the correct way to handle requesting a nonexistant order.
    // Right now this test is just here to keep it in mind. 
    [TestMethod]
    public async Task GetOrderFromNonExsistantRow()
    {
        // Arrange 
        string requestUri = "/api/Order/4";

        //Act 
        var response = await _client.GetAsync(requestUri);
        var responseContent = await response.Content.ReadAsStringAsync();

        //Assert
        Assert.AreEqual("Sequence contains no elements", responseContent);
    }
    [TestMethod]
    public async Task CreateOrder()
    {
        // Arrange
        using (var ctx = new StorageContext())
        {
            // Arrange
            OrderDTO dto = new OrderDTO()
            {
                Quantity = 5,
                Discount = 0.0m,
                Price = 100.0m,
                ProductID = 1,
                OrderListID = 0,
                Customer = new Customer()
                {
                    ID = 1,
                    Name = "John",
                    Email = "John@doe.com",
                    Address = "Teststreet 7",
                    Type = 1 // Customer
                }
            };

            var customer = dto.Customer;

            // Act
            var json = JsonSerializer.Serialize(dto);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _client.PostAsync("/api/Order", content);
            var responseContent = await response.Content.ReadAsStringAsync();

            //Assert
            response.EnsureSuccessStatusCode();
            responseContent.Contains(json);
            Assert.IsTrue(responseContent.Contains(customer.Name), "Response content should contain the customer name.");
            Assert.IsTrue(responseContent.Contains(customer.Address), "Response content should contain the customer address.");
            Assert.IsTrue(responseContent.Contains(customer.Email), "Response content should contain the customer email.");
        }
    }

}
