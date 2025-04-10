using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StorageSystem.Helpers
{
    public enum TransactionType
    {
        Sale,
        Return,
        StockRefill
    }

    public enum CustomerType
    {
        Normal,
        Business,
        //Admin
    }
}
