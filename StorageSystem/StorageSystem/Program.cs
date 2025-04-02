using StorageSystem;
using StorageSystem.Models;

class Program
{
    static void Main(string[] args)
    {
        using (var context = new StorageContext())
        {
            context.Database.EnsureCreated();

            context.SaveChanges();
        }

        Console.WriteLine("Hello World!");
    }
}