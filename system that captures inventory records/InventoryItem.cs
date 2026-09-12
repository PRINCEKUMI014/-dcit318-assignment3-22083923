using System;

namespace InventoryAppNamespace;

public record InventoryItem(int Id, string Name, int Quantity, DateTime DateAdded) : IInventoryEntity;
