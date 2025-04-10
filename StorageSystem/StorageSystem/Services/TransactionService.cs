using Microsoft.EntityFrameworkCore;
using StorageSystem.Models;

namespace StorageSystem.Services
{
    public class TransactionService
    {
        static public Transaction? Get(int id)
        {
            using (var ctx = new StorageContext())
            {
                return ctx.Transactions.SingleOrDefault(t => t.ID == id);
            }
        }

        static public ICollection<Transaction>? Get()
        {
            using var ctx = new StorageContext();
            return [.. ctx.Transactions];
        }

        static public Transaction Create(OrderList orderList, Helpers.TransactionType type)
        {
            using var ctx = new StorageContext();

            // Check to see if the order list already have a transaction.
            // If it does, return it.
            Transaction? tx = ctx.Transactions.Where(t => t.OrderListID == orderList.ID).SingleOrDefault();
            if (tx != null)
                return tx;

            tx = new Transaction { OrderListID = orderList.ID, Type = type, Date = DateTime.Now };
            ctx.Transactions.Add(tx);
            ctx.SaveChanges();
            return tx;
        }
    
        static public Transaction CreateSale(OrderList orderList)
        {
            return Create(orderList, Helpers.TransactionType.Sale);
        }

        static public Transaction CreateReturn(OrderList orderList)
        {
            return Create(orderList, Helpers.TransactionType.Return);
        }

        static public Receipt CreateReceipt(Transaction tx)
        {
            using var ctx = new StorageContext();
            Receipt receipt = new Receipt { TransactionID = tx.ID, Date = DateTime.Now };
            ctx.Receipts.Add(receipt);
            ctx.SaveChanges();
            return receipt;
        }
    }
}
