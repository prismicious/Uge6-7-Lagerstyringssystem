using Microsoft.EntityFrameworkCore;
using StorageSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StorageSystem.Services
{
    internal class TransactionService
    {
        static Transaction? GetTransactionByID(int id)
        {
            using (var ctx = new StorageContext())
            {
                return ctx.Transactions.SingleOrDefault(t => t.ID == id);
            }
            
        }
        static ICollection<Transaction>? GetWarehouseTransactions(int id)
        {
            using (var ctx = new StorageContext())
            {
                Warehouse? warehouse = ctx.Warehouses.Include(w => w.Transactions).FirstOrDefault(w => w.ID == id);

                if (warehouse != null)
                {
                    ICollection<Transaction> result = warehouse.Transactions;
                    return result;
                } else
                return null;
            }
        }

        static 

    }
}
