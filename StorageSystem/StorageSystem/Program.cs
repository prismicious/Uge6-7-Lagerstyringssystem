using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using StorageSystem.Models;

public class StorageContext : DbContext
{
    public DbSet<Product> Products { get; set; }

    public string DbPath { get; }

    public StorageContext()
    {
        var folder = Environment.SpecialFolder.LocalApplicationData;
        var path = Environment.GetFolderPath(folder);
        DbPath = System.IO.Path.Join(path, "storage.db");
    }

    // The following configures EF to create a Sqlite database file in the
    // special "local" folder for your platform.
    protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseSqlite($"Data Source={DbPath}");
}
