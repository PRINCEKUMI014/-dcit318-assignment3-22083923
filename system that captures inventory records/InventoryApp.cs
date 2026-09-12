using System;

namespace InventoryAppNamespace;

public class InventoryApp
{
    private readonly InventoryLogger<InventoryItem> _logger;

    public InventoryApp(string filePath = "inventory.json")
    {
        _logger = new InventoryLogger<InventoryItem>(filePath);
    }

    public void SeedSampleData()
    {
        _logger.Add(new InventoryItem(1, "Keyboard", 10, DateTime.UtcNow));
        _logger.Add(new InventoryItem(2, "Monitor", 5, DateTime.UtcNow.AddMinutes(-10)));
        _logger.Add(new InventoryItem(3, "Mouse", 25, DateTime.UtcNow.AddHours(-2)));
        _logger.Add(new InventoryItem(4, "Webcam", 7, DateTime.UtcNow.AddDays(-1)));
        _logger.Add(new InventoryItem(5, "Headset", 12, DateTime.UtcNow.AddDays(-3)));
    }

    public void SaveData() => _logger.SaveToFile();

    public void LoadData() => _logger.LoadFromFile();

    public void PrintAllItems()
    {
        var items = _logger.GetAll();
        if (items.Count == 0)
        {
            Console.WriteLine("No items found.");
            return;
        }

        foreach (var item in items)
        {
            Console.WriteLine($"Id: {item.Id}, Name: {item.Name}, Quantity: {item.Quantity}, DateAdded: {item.DateAdded:O}");
        }
    }
}
