using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using StorageSystem.Models;

namespace StorageSystem.Services
{
    class CustomerService 
    {
        public static Customer? getCustomer(int id)
        {
            using (var ctx = new StorageContext())
            {
                return ctx.Customers.Where(p => p.ID == id).SingleOrDefault();
            }
        }
        public static List<Receipt>? getCustomerReceipts(int id)
        {
            using (var ctx = new StorageContext())
            {
                return ctx.Customers
                    .Where(c => c.ID ==  id)
                    .SelectMany(c => c.OrderLists)
                    .Select(t => t.Transaction)
                    .Where()
            }
        }

        public static void deleteCustomer()
        {

        }

        public static Customer? createCustomer()
        {

        }

    }
}
