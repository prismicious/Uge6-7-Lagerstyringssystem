namespace StorageSystem.Models
{
    public class OrderList
    {
        public int ID { get; set; }
        public ICollection<Order> Orders { get; set; }

        public int TransactionID { get; set; }
        public Transaction Transaction { get; set; }

        public int CustomerID { get; set; }
        public Customer Customer { get; set; }
    }
}
