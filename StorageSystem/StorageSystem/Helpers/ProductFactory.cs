using StorageSystem.Interfaces;
using StorageSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StorageSystem.Helpers
{
    public class ProductFactory
    {
        public static IProduct Create(string type, string name, decimal price)
        {
            switch (type)
            {
                case "ProductA":
                    return new ProductA(type, name, price);
                case "ProductB":
                    return new ProductB(type, name, price);
                default:
                    throw new NotSupportedException($"{type} is not supported");
            }
        }
    }
}
