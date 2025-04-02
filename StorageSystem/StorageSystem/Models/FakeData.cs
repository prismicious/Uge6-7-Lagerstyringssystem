using Bogus;
using StorageSystem;
using StorageSystem.Models;

public static class FakeData
{
    public static List<Product> Products { get; set; } = new List<Product>();
    public static List<Customer> Customers { get; set; } = new List<Customer>();
}