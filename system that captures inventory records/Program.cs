using System;
using InventoryAppNamespace;

class Program
{
    static void Main()
    {
        Console.WriteLine("Inventory app starting...");

        string filePath = "inventory.json";

        // Seed and save data
        var app = new InventoryApp(filePath);
        app.SeedSampleData();
        app.SaveData();

        // Simulate a new session by creating a new app instance and loading from disk
        var newSessionApp = new InventoryApp(filePath);
        newSessionApp.LoadData();
        newSessionApp.PrintAllItems();

        Console.WriteLine("Done.");
    }
}
