using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StorageSystem.Interfaces
{
    //If we want to require the products have implemented a constructor then we need to change the interface to an abstract class. 
    public interface IProduct
    {
        
        public int ID { get; set; }
        public decimal Price { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
    }
}
