using StorageSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StorageSystem.Strategies
{
    interface IPricingStrategy
    {
        abstract static decimal Calculate(OrderList ol, Product p);
    }

    public class BigOrderStrategy : IPricingStrategy
    {
        public static decimal Calculate(OrderList ol, Product p)
        {
            // 10% off for orders over $100
            decimal discount = 0;
            if (ol.Orders.Sum(o => o.Quantity * p.Price) > 100)
            {
                discount = 0.10m; // 10% discount
            }
            return p.Price - (p.Price * discount);
        }
    }
}
