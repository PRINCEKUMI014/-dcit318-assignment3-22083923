using System;
using System.Collections.Generic;

namespace WarehouseInventoryManagementSystem
{
    // Marker interface
    public interface IInventoryItem
    {
        int Id { get; }
        string Name { get; }
        int Quantity { get; set; }
    }

    // Electronic item
    public class ElectronicItem : IInventoryItem
    {
        public int Id { get; }
        public string Name { get; }
        public int Quantity { get; set; }
        public string Brand { get; }
        public int WarrantyMonths { get; }

        public ElectronicItem(int id, string name, int quantity, string brand, int warrantyMonths)
        {
            Id = id;
            Name = name;
            Quantity = quantity;
            Brand = brand;
            WarrantyMonths = warrantyMonths;
        }

        public override string ToString()
        {
            return $"[Electronic] Id={Id}, Name={Name}, Quantity={Quantity}, Brand={Brand}, WarrantyMonths={WarrantyMonths}";
        }
    }

    // Grocery item
    public class GroceryItem : IInventoryItem
    {
        public int Id { get; }
        public string Name { get; }
        public int Quantity { get; set; }
        public DateTime ExpiryDate { get; }

        public GroceryItem(int id, string name, int quantity, DateTime expiryDate)
        {
            Id = id;
            Name = name;
            Quantity = quantity;
            ExpiryDate = expiryDate;
        }

        public override string ToString()
        {
            return $"[Grocery] Id={Id}, Name={Name}, Quantity={Quantity}, Expiry={ExpiryDate:yyyy-MM-dd}";
        }
    }

    // Custom exceptions
    public class DuplicateItemException : Exception
    {
        public DuplicateItemException(string message) : base(message) { }
    }

    public class ItemNotFoundException : Exception
    {
        public ItemNotFoundException(string message) : base(message) { }
    }

    public class InvalidQuantityException : Exception
    {
        public InvalidQuantityException(string message) : base(message) { }
    }

    // Generic repository
    public class InventoryRepository<T> where T : IInventoryItem
    {
        private readonly Dictionary<int, T> _items = new Dictionary<int, T>();

        public void AddItem(T item)
        {
            if (_items.ContainsKey(item.Id))
                throw new DuplicateItemException($"An item with Id {item.Id} already exists.");

            _items[item.Id] = item;
        }

        public T GetItemById(int id)
        {
            if (!_items.TryGetValue(id, out var item))
                throw new ItemNotFoundException($"Item with Id {id} was not found.");
            return item;
        }

        public void RemoveItem(int id)
        {
            if (!_items.Remove(id))
                throw new ItemNotFoundException($"Item with Id {id} was not found and could not be removed.");
        }

        public List<T> GetAllItems()
        {
            return new List<T>(_items.Values);
        }

        public void UpdateQuantity(int id, int newQuantity)
        {
            if (newQuantity < 0)
                throw new InvalidQuantityException($"Quantity cannot be negative: {newQuantity}");

            var item = GetItemById(id); // will throw ItemNotFoundException if missing
            item.Quantity = newQuantity;
        }
    }

    // Warehouse manager
    public class WareHouseManager
    {
        public InventoryRepository<ElectronicItem> Electronics { get; } = new InventoryRepository<ElectronicItem>();
        public InventoryRepository<GroceryItem> Groceries { get; } = new InventoryRepository<GroceryItem>();

        public void SeedData()
        {
            // Electronics
            try
            {
                Electronics.AddItem(new ElectronicItem(1, "Smartphone", 50, "Acme", 24));
                Electronics.AddItem(new ElectronicItem(2, "Laptop", 20, "Contoso", 12));
                Electronics.AddItem(new ElectronicItem(3, "Headphones", 100, "Acme", 6));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Seed electronics error: {ex.Message}");
            }

            // Groceries
            try
            {
                Groceries.AddItem(new GroceryItem(101, "Milk", 200, DateTime.Today.AddDays(7)));
                Groceries.AddItem(new GroceryItem(102, "Bread", 150, DateTime.Today.AddDays(3)));
                Groceries.AddItem(new GroceryItem(103, "Eggs", 500, DateTime.Today.AddDays(14)));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Seed groceries error: {ex.Message}");
            }
        }

        public void PrintAllItems<T>(InventoryRepository<T> repo) where T : IInventoryItem
        {
            var list = repo.GetAllItems();
            if (list.Count == 0)
            {
                Console.WriteLine("No items to display.");
                return;
            }

            foreach (var item in list)
            {
                Console.WriteLine(item.ToString());
            }
        }

        public void IncreaseStock<T>(InventoryRepository<T> repo, int id, int quantity) where T : IInventoryItem
        {
            try
            {
                var item = repo.GetItemById(id);
                var newQuantity = checked(item.Quantity + quantity);
                repo.UpdateQuantity(id, newQuantity);
                Console.WriteLine($"Increased stock for Id {id}. New quantity: {newQuantity}");
            }
            catch (DuplicateItemException dex)
            {
                Console.WriteLine($"Duplicate item error: {dex.Message}");
            }
            catch (ItemNotFoundException inf)
            {
                Console.WriteLine($"Not found: {inf.Message}");
            }
            catch (InvalidQuantityException iq)
            {
                Console.WriteLine($"Invalid quantity: {iq.Message}");
            }
            catch (OverflowException)
            {
                Console.WriteLine("Quantity overflow occurred when increasing stock.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error increasing stock: {ex.Message}");
            }
        }

        public void RemoveItemById<T>(InventoryRepository<T> repo, int id) where T : IInventoryItem
        {
            try
            {
                repo.RemoveItem(id);
                Console.WriteLine($"Removed item with Id {id} successfully.");
            }
            catch (ItemNotFoundException inf)
            {
                Console.WriteLine($"Cannot remove item: {inf.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error removing item: {ex.Message}");
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var manager = new WareHouseManager();
            manager.SeedData();

            Console.WriteLine("\n-- Grocery Items --");
            manager.PrintAllItems(manager.Groceries);

            Console.WriteLine("\n-- Electronic Items --");
            manager.PrintAllItems(manager.Electronics);

            Console.WriteLine("\n-- Exception Scenarios --");

            // 1) Add a duplicate item
            try
            {
                Console.WriteLine("Attempting to add a duplicate electronic item (Id=1)...");
                manager.Electronics.AddItem(new ElectronicItem(1, "DuplicatePhone", 10, "Acme", 12));
            }
            catch (DuplicateItemException ex)
            {
                Console.WriteLine($"Caught duplicate item: {ex.Message}");
            }

            // 2) Remove a non-existent item
            try
            {
                Console.WriteLine("Attempting to remove non-existent grocery item (Id=999)...");
                manager.Groceries.RemoveItem(999);
            }
            catch (ItemNotFoundException ex)
            {
                Console.WriteLine($"Caught item not found: {ex.Message}");
            }

            // 3) Update with invalid quantity
            try
            {
                Console.WriteLine("Attempting to set invalid quantity (-5) for electronic item (Id=2)...");
                manager.Electronics.UpdateQuantity(2, -5);
            }
            catch (InvalidQuantityException ex)
            {
                Console.WriteLine($"Caught invalid quantity: {ex.Message}");
            }

            // Demonstrate IncreaseStock and RemoveItemById with error handling
            Console.WriteLine();
            manager.IncreaseStock(manager.Groceries, 101, 50); // valid
            manager.IncreaseStock(manager.Groceries, 999, 10); // non-existent
            manager.RemoveItemById(manager.Electronics, 999); // non-existent

            Console.WriteLine("\nDone.");
        }
    }
}
