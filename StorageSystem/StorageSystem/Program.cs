﻿using StorageSystem;
using StorageSystem.Models;
using StorageSystem.Services;

class Program
{
    static void Main(string[] args)
    {
        LogService.Log("Application started.");
        FakerService.Generate();
    }
}
