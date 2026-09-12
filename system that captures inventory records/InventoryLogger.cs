using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace InventoryAppNamespace;

public class InventoryLogger<T> where T : IInventoryEntity
{
    private readonly List<T> _log = new();
    private readonly string _filePath;

    public InventoryLogger(string filePath)
    {
        _filePath = string.IsNullOrWhiteSpace(filePath) ? "inventory.json" : filePath;
    }

    public void Add(T item)
    {
        if (item == null) throw new ArgumentNullException(nameof(item));
        _log.Add(item);
    }

    public List<T> GetAll()
    {
        return new List<T>(_log);
    }

    public void SaveToFile()
    {
        try
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            string json = JsonSerializer.Serialize(_log, options);
            using var writer = new StreamWriter(_filePath, false);
            writer.Write(json);
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error saving to file '{_filePath}': {ex.Message}");
        }
    }

    public void LoadFromFile()
    {
        try
        {
            if (!File.Exists(_filePath))
            {
                _log.Clear();
                return;
            }

            using var reader = new StreamReader(_filePath);
            string json = reader.ReadToEnd();
            var items = JsonSerializer.Deserialize<List<T>>(json);
            _log.Clear();
            if (items != null) _log.AddRange(items);
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error loading from file '{_filePath}': {ex.Message}");
        }
    }
}
