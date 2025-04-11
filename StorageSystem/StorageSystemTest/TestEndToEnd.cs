using StorageSystem;
using StorageSystem.Helpers;
using StorageSystem.Models;
using StorageSystem.Services;
namespace StorageSystemTest;

[TestClass]
public class TestEndToEnd
{
    [TestInitialize]
    public void Init()
    {
        // Reset the database
        using var ctx = new StorageContext();
        ctx.Database.EnsureDeleted();
        ctx.Database.EnsureCreated();
    }

    [TestMethod]
    public void EndToEnd()
    {
        // Create a customer
        var customer = CustomerService.Create(
            "Knud Karate",
            "cirkelspark@ansigt.dk",
            "Kindhestegade 23",
            CustomerType.Normal);
        Assert.IsNotNull(customer);

        // Create an example product
        var product = ProductService.Create(2.34m, "Product Name", "Product Type");

        // Create a product status for the product.
        // Add 5x product to the stock.
        var status = ProductStatusService.Create(product, 5);
        Assert.IsNotNull(status);
        Assert.AreEqual(5, ProductStatusService.Get(product)?.Quantity);

        // Create an order list for the customer, with 1 order on it.
        // This orders more product than is in stock.
        var orderlist = OrderListService.Create(customer);
        var order = OrderService.Create(orderlist, product, 10);

        // Not enough stock to fulfill the order,
        // so order some more.
        {
            ProductStatusService.OrderMoreStock(product, 5);
            Assert.AreEqual(10, ProductStatusService.Get(product)?.Quantity);
        }

        // Warehouse now has enough product,
        // so create the final transaction
        var tx = TransactionService.CreateSale(orderlist);

        // When the warehouse ships out the product,
        // create a receipt for the transaction.
        var receipt = TransactionService.CreateReceipt(tx);
    }
}
