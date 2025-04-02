namespace StorageSystem.Models
{
    public class Warehouse
    {
        public int ID { get; set; }
        public string Location { get; set; }

        public ICollection<ProductStatus> ProductStatuses { get; set; }
        public ICollection<Transaction> Transactions { get; set; }
    }
}
