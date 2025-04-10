using StorageSystem.Helpers;

namespace StorageSystem.Models
{
    public class Customer
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public CustomerType Type { get; set; } // Customer, Business, Warehouse

        public ICollection<OrderList> OrderLists;
        public ICollection<Receipt> Receipts;
    }
}
