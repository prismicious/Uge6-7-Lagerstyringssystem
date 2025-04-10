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
        var customer = CustomerService.Create("Knud Karate", "cirkelspark@ansigt.dk", "Kindhestegade 23", CustomerType.Normal);
        Assert.IsNotNull(customer);


        // Create an example product
        var product = ProductService.Create(2.34m, "Product Name", "Product Type");


        // Create two warehouses
        var warehouse_1 = WarehouseService.Create("Aarhus");
        var warehouse_2 = WarehouseService.Create("Ballerup");


        // Create a product status for the product in both warehouses.
        // Add five to each warehouse.
        var status_1 = WarehouseService.CreateProductStatus(warehouse_1, product, 5);
        var status_2 = WarehouseService.CreateProductStatus(warehouse_2, product, 5);

        Assert.IsNotNull(status_1);
        Assert.IsNotNull(status_2);
        Assert.AreEqual(5, WarehouseService.GetProductStatus(warehouse_2, product)?.Quantity);
        Assert.AreEqual(5, WarehouseService.GetProductStatus(warehouse_1, product)?.Quantity);


        // Create an orderlist for the customer, with 1 order on it.
        // This orders more product than exist in either warehouse.
        var orderlist = OrderListService.Create(customer);
        var order = OrderService.Create(orderlist, product, 10, 0, product.Price);

        

        // Neither warehouse has enough stock to fulfill the order,
        // so move product from warehouse 2 to warehouse 1
        {
            var orderMove = OrderListService.CreateForWarehouseMove(warehouse_1);
            OrderService.Create(orderMove, product, status_2.Quantity, 0, 0);
            WarehouseService.MoveProduct(warehouse_2, warehouse_1, orderMove);

            Assert.AreEqual( 0, WarehouseService.GetProductStatus(warehouse_2, product)?.Quantity);
            Assert.AreEqual(10, WarehouseService.GetProductStatus(warehouse_1, product)?.Quantity);
        }

        // Warehouse now has enough product,
        // so create the final transaction
        var tx = TransactionService.CreateSale(orderlist);


        // Create a receipt for the transaction.
        // This happens when the warehouse ships out the product.
        var receipt = TransactionService.CreateReceipt(tx);
    }
}
