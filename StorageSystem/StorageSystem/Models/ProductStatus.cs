using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StorageSystem.Models
{
    public class ProductStatus
    {
        public int ID { get; set; }
        public int Quantity { get; set; }
        public int Reserved { get; set; }

        public int ProductID { get; set; }
        public Product Product { get; set; }
        public Warehouse Warehouse { get; set; }
    }
}
