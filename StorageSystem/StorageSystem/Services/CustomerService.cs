using StorageSystem.Models;
using StorageSystem.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StorageSystem.Services
{
    public class CustomerService 
    {
        public static Customer? Get(int id)
        {
            using (var ctx = new StorageContext())
            {
                return ctx.Customers.Where(c => c.ID == id).SingleOrDefault();
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
        public static bool Update(Customer customer)
        {
            if (customer.ID == 0 || Get(customer.ID) == null)
            {
                return false;
            }
            using (var ctx = new StorageContext())
            {
                ctx.Customers.Update(customer);
                return 0 != ctx.SaveChanges();
            }
        }

        public static Customer? Create(string name, string email, string address, CustomerType type)
        {
            using (var ctx = new StorageContext())
            {
                Customer customer = new Customer { Name = name, Email = email, Address = address, Type = type };
                if (null != ctx.Customers.Where(c => c.Email == email).SingleOrDefault())
                {
                    return null;
                }
                ctx.Customers.Add(customer);
                ctx.SaveChanges();
                return customer;
            }
        }
        
        public static bool Delete(int ID) 
        {
            if (ID == 0)
                return false;
            Customer? customer = Get(ID);
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
