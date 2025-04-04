using StorageSystem.Helpers;

namespace StorageSystem.Models
{
    public class Transaction
    {
        public int ID { get; set; }
        public DateTime Date { get; set; }
        public TransactionType Type { get; set; }

        public int OrderListID { get; set; }
        public OrderList OrderList { get; set; }
        public Receipt Receipt { get; set; }
        public Warehouse Warehouse { get; set; }
    }
}