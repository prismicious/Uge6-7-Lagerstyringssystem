using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
