using System;
using System.Collections.Generic;
using StorageSystem;
class Program
{
    static void Main(string[] args)
    {
        using (var context = new StorageContext())
        {
            context.Database.EnsureCreated();
        }

        Console.WriteLine("Hello World!");
    }
}