using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StorageSystem.Models
{
    public class Product
    {
        public int ID { get; set; }
        public decimal Price { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }

        public ProductStatus ProductStatus { get; set; }
    }
}
