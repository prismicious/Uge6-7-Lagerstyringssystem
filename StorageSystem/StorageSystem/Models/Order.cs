namespace StorageSystem.Models
{
    public class Order
    {
        public int ID { get; set; }
        public int Quantity { get; set; }
        public decimal Discount { get; set; }
        public decimal Price { get; set; }

        public Product Product { get; set; }
        public OrderList OrderList { get; set; }
    }
}
