namespace StorageSystem.Models
{
    public class Receipt
    {
        public int ID { get; set; }
        public DateTime Date { get; set; }

        public int TransactionID { get; set; }
        public Transaction Transaction { get; set; }
    }
}
