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
            var warehouse1 = new Warehouse { ID = 1,Location = "Jylland",Transactions = new List<Transaction>()};
            var transaction1 = new Transaction
            {
                ID = 1,
                Date = DateTime.Now,
                Type = TransactionType.Sale,
                Warehouse = warehouse1  
            };
            var transaction2 = new Transaction
            {
                ID = 2,
                Date = DateTime.Now.AddDays(-1),
                Type = TransactionType.Transfer,
                Warehouse = warehouse1  
            };
            var transaction3 = new Transaction
            {
                ID = 3,
                Date = DateTime.Now.AddDays(-2),
                Type = TransactionType.Transfer,
                Warehouse = warehouse1
            };
            warehouse1.Transactions.Add(transaction1);
            warehouse1.Transactions.Add(transaction2);
            warehouse1.Transactions.Add(transaction3);
            ctx.Add(warehouse1);
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
