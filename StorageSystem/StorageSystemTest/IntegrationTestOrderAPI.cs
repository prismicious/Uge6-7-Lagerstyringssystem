using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using StorageSystem;
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
            Customer customer = new Customer() { ID = 1 , Name = "John",Email = "JohnMadden@handegg.com",Address = "USA"};
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


        
        Order order;
        Order order1;
        // Arrange
        using (var ctx = new StorageContext())
        {
            Customer customer = new Customer() { ID = 2, Name = "Mohn", Email = "MohnJadden@egghand.com", Address = "ASU" };
            ctx.Customers.Add(customer);
            ctx.SaveChanges();
            OrderList orderlist = OrderListService.Create(customer);
            Product product = ProductService.Create(100.0m, "Test", "Type");

            //order1 = OrderService.Create(orderlist, product, 10, 0, 15);
            order = new Order() { ID = 4, Price = 100.0m, Product = product, Discount = 0.0m, OrderList = orderlist, Quantity = 5, ProductID = 2,OrderListID = 2};
            string RequestUri = "/api/order";
            var content = new StringContent(JsonSerializer.Serialize(order), Encoding.UTF8, "application/json");
            Console.WriteLine("HEJSA JEG ER HER");
            string my_content = await content.ReadAsStringAsync();
            Console.WriteLine(my_content);
            //Act 
            var response = await _client.PostAsync(RequestUri, content);
            Console.WriteLine(response);
            //var responseContent = await response.Content.ReadAsStringAsync();

            //Assert
            response.EnsureSuccessStatusCode();
        }
    }

}
