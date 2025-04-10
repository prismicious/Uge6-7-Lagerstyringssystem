using System.ComponentModel.DataAnnotations;
using StorageSystem.Models;

namespace StorageSystem.DTOs
{
    public class OrderDTO
    {
        public int ID { get; set; }

        [Required (ErrorMessage = "Quantity is required")]
        [Range(1, 10, ErrorMessage = "Quantity must be greater than 0")]
        public int Quantity { get; set; }
        public decimal Discount { get; set; }

        [Required (ErrorMessage = "Price is required")]
        public decimal Price { get; set; }

        [Required (ErrorMessage = "Product ID is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Product ID must be greater than 0")]
        public int ProductID { get; set; }
        public int OrderListID { get; set; }

        [Required (ErrorMessage = "Customer is required")]
        public int CustomerID { get; set; }
    }
}