using Microsoft.EntityFrameworkCore;
using StorageSystem.Models;

namespace StorageSystem
{
    public class StorageContext : DbContext
    {
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductStatus> ProductStatuses { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Warehouse> Warehouses { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<OrderList> OrderLists { get; set; }
        public DbSet<Receipt> Receipts { get; set; }


        // The following configures EF to create a Sqlite database.
        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite("Data Source=storage.sqlite");
    }
}
