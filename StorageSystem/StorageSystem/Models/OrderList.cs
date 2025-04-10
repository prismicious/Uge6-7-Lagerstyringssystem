namespace StorageSystem.Models
{
    public class OrderList
    {
        public int ID { get; set; }

        // Collection navigation containing dependents
        public ICollection<Order> Orders { get; } = new List<Order>();

        // Optional reference navigation. May not exist yet.
        public Transaction? Transaction { get; set; }

        // Customer foreign key
        public int CustomerID { get; set; }
        // Customer reference navigation
        public Customer Customer { get; set; }
    }
}
