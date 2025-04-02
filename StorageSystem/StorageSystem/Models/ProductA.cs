using StorageSystem.Interfaces;

namespace StorageSystem.Models
{
    //Just an example showing how an implementation of IProduct would look
    public class ProductA(string type, string name, decimal price) : IProduct
    {
        public int ID { get; set; }
        public decimal Price { get; set; } = price;
        public string Name { get; set; } = name;
        public string Type { get; set; } = type;
    }
}
