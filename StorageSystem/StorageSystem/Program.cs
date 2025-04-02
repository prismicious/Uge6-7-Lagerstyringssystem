using StorageSystem;
using StorageSystem.Models;
using StorageSystem.Services;

class Program
{
    static void Main(string[] args)
    {
        int productsToGenerate = 100;
        int customersToGenerate = 20;
        using (var context = new StorageContext())
        {
            bool isCreated = context.Database.EnsureCreated();
            if (isCreated)
                FakerService.GenerateAndPopulate(context, productsToGenerate, customersToGenerate);
        }

        Console.WriteLine($"Inserted {productsToGenerate} generated products and {customersToGenerate} generated customers into the database.");
    }
}
