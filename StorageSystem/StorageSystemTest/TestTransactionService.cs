using StorageSystem;
using StorageSystem.Helpers;
using StorageSystem.Models;
using StorageSystem.Services;
namespace StorageSystemTest;

[TestClass]
public class TestTransactionService
{
    [TestInitialize]
    public void init()
    {
        using (var ctx = new StorageContext())
        {
            ctx.Database.EnsureDeleted();
            ctx.Database.EnsureCreated();

            var warehouse1 = new Warehouse { Location = "Jylland" };
            ctx.Warehouses.Add(warehouse1);
            ctx.SaveChanges();

            var customer = new Customer { Name = "warehouse:1", Email = "test", Address = "test", Type = 2 };
            ctx.Customers.Add(customer);
            ctx.SaveChanges();

            var ol1 = OrderListService.Create(customer);
            var ol2 = OrderListService.Create(customer);
            var ol3 = OrderListService.Create(customer);

            var transaction1 = new Transaction
            {
                ID = 1,
                Date = DateTime.Now,
                Type = TransactionType.Sale,
                OrderListID = ol1.ID,
                WarehouseID = warehouse1.ID
            };
            var transaction2 = new Transaction
            {
                ID = 2,
                Date = DateTime.Now.AddDays(-1),
                Type = TransactionType.Return,
                OrderListID = ol2.ID,
                WarehouseID = warehouse1.ID
            };
            var transaction3 = new Transaction
            {
                ID = 3,
                Date = DateTime.Now.AddDays(-2),
                Type = TransactionType.Transfer,
                OrderListID = ol3.ID,
                WarehouseID = warehouse1.ID
            };


            ctx.Transactions.Add(transaction1);
            ctx.Transactions.Add(transaction2);
            ctx.Transactions.Add(transaction3);
            ctx.SaveChanges();

            Assert.AreEqual(1, ctx.Warehouses.Count());
            Assert.AreEqual(3, ctx.Transactions.Count());

        }
    }

    [TestMethod]
    public void TestGetTransactionFromID()
    {
        var result = TransactionService.Get(1);

        Assert.AreEqual(TransactionType.Sale, result?.Type);

    }
    [TestMethod]
    public void TestGetAllWareTransactions()
    {
        var result = TransactionService.GetWarehouseTransactions(1);

        Assert.AreEqual(3, result?.Count());
    }
}
