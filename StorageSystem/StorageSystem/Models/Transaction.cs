using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StorageSystem.Models
{
    public class Transaction
    {
        public int ID { get; set; }
        public DateTime Date { get; set; }
        public int Type { get; set; }

        public OrderList OrderList { get; set; }
        public Receipt Receipt { get; set; }
        public Warehouse Warehouse { get; set; }
    }
}
