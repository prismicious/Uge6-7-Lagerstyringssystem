using System.ComponentModel.DataAnnotations;

namespace StorageSystem.Models
{
    public class Product
    {
        public int ID { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        [MinLength(1, ErrorMessage = "Name cannot be empty.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Price is required.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than zero.")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Type is required.")]
        [MinLength(1, ErrorMessage = "Type cannot be empty.")]
        public string Type { get; set; }
    }
}
