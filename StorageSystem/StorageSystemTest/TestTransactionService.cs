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

            var customer = CustomerService.Create("A", "A", "A", CustomerType.Normal);

            var ol1 = OrderListService.Create(customer.ID);
            var ol2 = OrderListService.Create(customer.ID);
            var ol3 = OrderListService.Create(customer.ID);

            var transaction1 = new Transaction
            {
                ID = 1,
                Date = DateTime.Now,
                Type = TransactionType.Sale,
                OrderListID = ol1.ID
            };
            var transaction2 = new Transaction
            {
                ID = 2,
                Date = DateTime.Now.AddDays(-1),
                Type = TransactionType.Return,
                OrderListID = ol2.ID
            };
            var transaction3 = new Transaction
            {
                ID = 3,
                Date = DateTime.Now.AddDays(-2),
                Type = TransactionType.StockRefill,
                OrderListID = ol3.ID
            };


            ctx.Transactions.Add(transaction1);
            ctx.Transactions.Add(transaction2);
            ctx.Transactions.Add(transaction3);
            ctx.SaveChanges();

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
    public void TestGetAllTransactions()
    {
        var result = TransactionService.Get();
        Assert.AreEqual(3, result?.Count());
    }
}
