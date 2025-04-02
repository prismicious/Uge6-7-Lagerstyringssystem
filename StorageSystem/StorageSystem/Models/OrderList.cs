using Microsoft.EntityFrameworkCore.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StorageSystem.Models
{
    public class OrderList
    {
        public int ID { get; set; }
        public ICollection<Order> Orders { get; set; }

        public int TransactionID { get; set; }
        public Transaction Transaction { get; set; }
    }
}
