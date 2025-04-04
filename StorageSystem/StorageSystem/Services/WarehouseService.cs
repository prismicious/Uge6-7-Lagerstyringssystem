using StorageSystem;
using StorageSystem.Models;

namespace StorageSystem.Services
{
    public static class WarehouseService
    {
        public static Warehouse Create(string location)
        {
            using var ctx = new StorageContext();
            var warehouse = new Warehouse { Location = location };
            ctx.Warehouses.Add(warehouse);
            ctx.SaveChanges();
            return warehouse;
        }

    }
}
