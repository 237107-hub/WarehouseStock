namespace WarehouseStock.Core.Domain;

/// <summary>Склад зберігання товарів.</summary>
public sealed class Warehouse
{
    public int Id { get; init; }

    public string Name { get; init; } = string.Empty;

    public string Address { get; init; } = string.Empty;
}
