using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using StorageSystem;
using StorageSystem.DTOs;
using StorageSystem.Models;
using StorageSystem.Services;
using StorageSystem.Helpers;


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
            Customer customer = new Customer() { Name = "John", Email = "JohnMadden@handegg.com", Address = "USA", Type = CustomerType.Normal };
            Customer wh_customer = new Customer() { Name = "warehouse:1", Email = "test", Address = "test", Type = CustomerType.Warehouse };
            ctx.Customers.Add(customer);
            ctx.Customers.Add(wh_customer);
            ctx.SaveChanges();
            Assert.IsTrue(customer.ID > 0, "Test customer was not created.");

            Product testProduct = ProductService.Create(1.0m, "TestProduct", "BestInTestTest");
            OrderList orderlist = OrderListService.Create(customer.ID);
            Order order1 = OrderService.Create(orderlist, testProduct, 50, 1, 100);
            Order order2 = OrderService.Create(orderlist, testProduct, 500, 1, 1000);
            Order order3 = OrderService.Create(orderlist, testProduct, 5000, 1, 2000);

            _testOrders = new List<Order> { order1, order2, order3 };
            OrderListService.AddOrders(orderlist, _testOrders);

            Assert.IsTrue(order1.ID > 0, "Test order was not created.");
            Assert.IsTrue(order2.ID > 0, "Test order was not created.");
            Assert.IsTrue(order3.ID > 0, "Test order was not created.");
            Assert.IsTrue(ctx.OrderLists.Any(ol => ol.ID == orderlist.ID), "Test order list was not created.");
        }
    }


    [TestMethod]
    public async Task GetAllOrders()
    {
        // Arrange 
        string requestUri = "/api/order";

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
        string requestUri = "/api/order/1";

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
        string requestUri = "/api/order/4";

        //Act 
        var response = await _client.GetAsync(requestUri);
        //Assert
        Assert.IsFalse(response.IsSuccessStatusCode, "Requesting a non-existent order should not be successful.");
    }
    [TestMethod]
    public async Task Create_ReturnsCreated()
    {
        using (var ctx = new StorageContext())
        {
            // Arrange
            var customer = ctx.Customers.FirstOrDefault(c => c.ID == 1);

            OrderDTO dto = new OrderDTO()
            {
                Quantity = 5,
                Discount = 0.0m,
                Price = 100.0m,
                ProductID = 1,
                OrderListID = 0,
                CustomerID = customer.ID
            };

            // Act
            var json = JsonSerializer.Serialize(dto);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _client.PostAsync("/api/Order", content);
            var responseContent = await response.Content.ReadAsStringAsync();

            //Assert
            response.EnsureSuccessStatusCode();
            responseContent.Contains(json);

            // Check if the customer was returned in the response
            Assert.IsTrue(responseContent.Contains(customer.ID.ToString()), "Response content should contain the customer ID.");

            // Check if the order was created in the database
            Assert.IsTrue(ctx.Orders.Any(o => o.Quantity == dto.Quantity && o.Discount == dto.Discount && o.Price == dto.Price), "Order was not created in the database.");
            Assert.IsTrue(ctx.OrderLists.Any(o => o.CustomerID == dto.CustomerID), "OrderList was not created in the database.");
        }
    }

    [TestMethod]
    public void CreateOrderWarehouseCustomer_ReturnsBadRequest()
    {
        // Arrange
        using (var ctx = new StorageContext())
        {
            OrderDTO orderDTO = new OrderDTO()
            {
                Quantity = 5,
                Discount = 0.0m,
                Price = 100.0m,
                ProductID = 1,
                OrderListID = 0,
                CustomerID = 2 // Assuming this is a warehouse customer ID
            };

            var json = JsonSerializer.Serialize(orderDTO);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var requestUri = "/api/order";

            // Act
            var response = _client.PostAsync(requestUri, content).Result;
            var responseContent = response.Content.ReadAsStringAsync().Result;

            // Assert
            Assert.IsFalse(response.IsSuccessStatusCode, "Order creation should fail for warehouse customers.");
            Assert.IsTrue(responseContent.Contains("Order creation is only allowed for regular customers."), "Response content should indicate that order creation is not allowed for warehouse customers.");
        }

    }
    [TestMethod]
    public async Task DeleteOrder_ReturnsNoContent()
    {
        // Arrange
        var orderToDelete = _testOrders.First();
        var requestUri = $"/api/order/{orderToDelete.ID}";

        // Act
        var response = await _client.DeleteAsync(requestUri);

        // Assert
        response.EnsureSuccessStatusCode();
        Assert.AreEqual(System.Net.HttpStatusCode.NoContent, response.StatusCode);

        using (var ctx = new StorageContext())
        {
            Assert.IsFalse(ctx.Orders.Any(o => o.ID == orderToDelete.ID), "Order was not deleted from the database.");
        }
    }

    

    [TestMethod]
    public async Task CreateOrder_InvalidInput_ReturnsBadRequest()
    {
        // Arrange
        var invalidOrder = new OrderDTO
        {
            Quantity = -5, // Invalid quantity
            Discount = 0.0m,
            Price = 100.0m,
            ProductID = 1,
            OrderListID = 0,
            CustomerID = 1
        };

        var json = JsonSerializer.Serialize(invalidOrder);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var requestUri = "/api/order";

        // Act
        var response = await _client.PostAsync(requestUri, content);

        // Assert
        Assert.AreEqual(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
    }

    [TestMethod]
    public async Task UpdateOrder_ReturnsNoContent()
    {
        // Arrange
        var orderToUpdate = _testOrders.First();
        var updatedOrderDTO = new OrderDTO
        {
            ID = orderToUpdate.ID,
            Quantity = 10, // Updated quantity
            Discount = 5.0m, // Updated discount
            Price = 200.0m, // Updated price
            ProductID = orderToUpdate.ProductID,
            OrderListID = orderToUpdate.OrderListID,
            CustomerID = orderToUpdate.OrderList.CustomerID
        };

        var json = JsonSerializer.Serialize(updatedOrderDTO);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var requestUri = $"/api/order/{orderToUpdate.ID}";

        // Act
        var response = await _client.PutAsync(requestUri, content);

        // Assert
        response.EnsureSuccessStatusCode();
        Assert.AreEqual(System.Net.HttpStatusCode.NoContent, response.StatusCode);

        using (var ctx = new StorageContext())
        {
            var updatedOrder = ctx.Orders.FirstOrDefault(o => o.ID == orderToUpdate.ID);
            Assert.IsNotNull(updatedOrder, "Updated order should exist in the database.");
            Assert.AreEqual(10, updatedOrder.Quantity, "Order quantity was not updated correctly.");
            Assert.AreEqual(5.0m, updatedOrder.Discount, "Order discount was not updated correctly.");
            Assert.AreEqual(200.0m, updatedOrder.Price, "Order price was not updated correctly.");
        }
    }

}
