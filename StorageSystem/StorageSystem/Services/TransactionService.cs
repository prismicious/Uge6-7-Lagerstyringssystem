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

        static public ICollection<Transaction>? GetWarehouseTransactions(int warehouseId)
        {
            using (var ctx = new StorageContext())
            {
                Warehouse? warehouse = ctx.Warehouses
                    .Include(w => w.Transactions)
                    .SingleOrDefault(w => w.ID == warehouseId);

                return warehouse?.Transactions;
            }
        }

        static public Transaction Create(OrderList orderList, Helpers.TransactionType type)
        {
            using var ctx = new StorageContext();

            // Check to see if the order list already have a transaction.
            // If it does, return it.
            Transaction? tx = ctx.Transactions.Where(t => t.OrderList == orderList).SingleOrDefault();
            if (tx != null)
                return tx;

            tx = new Transaction { OrderList = orderList, Type = type, Date = DateTime.Now };
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

        static public Transaction CreateWarehouseTransfer(OrderList orderList)
        {
            return Create(orderList, Helpers.TransactionType.Transfer);
        }
    }
}
