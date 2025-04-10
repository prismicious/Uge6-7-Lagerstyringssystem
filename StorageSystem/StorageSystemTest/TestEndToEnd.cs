using StorageSystem;
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
        //var warehouse_1 = WarehouseService.

        // Create a product status for the product in both warehouses

        // Create a customer

        // Select an appropiate warehouse for the customer

        // Create an orderlist for the customer

        // Create an order and add them to the orderlist

        // Create a transaction when the customer pays

        // Neither warehouse has enough stock to fulfill the order,
        // so move product from one warehouse to the other
        // - OR -
        // Create two transactions, one for each warehouse

        // Create a receipt for the transaction when the warehouse ships out the product
    }
}
