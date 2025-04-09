using StorageSystem.Models;

namespace StorageSystem.Services
{
    public static class CustomerService
    {
        public static Customer Create();
        public static Customer Get(int ID, bool includeOrderList = false, bool includeReceipts = false);
        public static List<Customer> Get(bool includeOrderList = false, bool includeReceipts = false);
        public static bool Update(Customer c);
        public static bool Delete(Customer c);
    }
}
