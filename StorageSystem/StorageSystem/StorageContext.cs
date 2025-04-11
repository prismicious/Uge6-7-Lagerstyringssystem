using Microsoft.EntityFrameworkCore;
using StorageSystem.Models;
using System.Xml;

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
        public DbSet<Customer> Customers { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite("Data Source=storage.sqlite");

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Only allow one customers with same email
            modelBuilder.Entity<Customer>()
                .HasIndex(c => c.Email)
                .IsUnique();

            // Only allow one product status per product
            modelBuilder.Entity<ProductStatus>()
                .HasIndex(ps => ps.ProductID)
                .IsUnique();
        }

    }
}
