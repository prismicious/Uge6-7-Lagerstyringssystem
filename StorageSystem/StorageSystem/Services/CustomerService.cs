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
        public static Customer? GetCustomer(int id)
        {
            using (var ctx = new StorageContext())
            {
                return ctx.Customers.Where(p => p.ID == id).SingleOrDefault();
            }
        }
        /*
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
        */
        public static bool UpdateCustomer(Customer customer)
        {
            if (customer.ID == 0 || GetCustomer(customer.ID) == null)
            {
                return false;
            }
            using (var ctx = new StorageContext())
            {
                ctx.Customers.Update(customer);
                return 0 != ctx.SaveChanges();
            }
        }

        public static Customer? CreateCustomer(string name, string email, string address, int type)
        {
            using (var ctx = new StorageContext())
            {
                Customer customer = new Customer { Name = name, Email = email, Address = address, Type = type };
                ctx.Customers.Add(customer);
                ctx.SaveChanges();
                return customer;
            }
        }
        
        public static bool DeleteCustomer(int ID) 
        {
            if (ID == 0)
                return false;
            Customer? customer = GetCustomer(ID);
            if (customer == null)
                return false;
            using (var ctx = new StorageContext())
            {
                ctx.Customers.Remove(customer);
                return 0 != ctx.SaveChanges();
            }
        }

    }
}
