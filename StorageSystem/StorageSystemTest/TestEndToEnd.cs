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
        // Create 1 example product
        var product = ProductService.Create(2.34m, "Example product", "-");

        // Create 2 warehouses
        var warehouse_1 = WarehouseService.Create("Aarhus");
        var warehouse_2 = WarehouseService.Create("Ballerup");

        // Create a product status for the product in both warehouses.
        // Add 5 to each warehouse.
        WarehouseService.CreateProductStatus(warehouse_1, product, 5);
        WarehouseService.CreateProductStatus(warehouse_2, product, 5);


        // Create a customer
        var customer = CustomerService.Create("Knud Karate", "cirkelspark@ansigt.dk", "Kindhestegade 23", CustomerType.Normal);
        Assert.IsNotNull(customer);

        // ** Select an appropiate warehouse for the customer

        // Create an orderlist for the customer
        var orderlist = OrderListService.Create(customer);

        // Create an order and add them to the orderlist
        var order = OrderService.Create(orderlist, product, 10, 0, product.Price);

        // Create a transaction when the customer pays

        // Neither warehouse has enough stock to fulfill the order,
        // so move product from one warehouse to the other
        // - OR -
        // Create two transactions, one for each warehouse

        // Create a receipt for the transaction when the warehouse ships out the product
    }
}
