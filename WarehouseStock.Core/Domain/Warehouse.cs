namespace WarehouseStock.Core.Domain;

/// <summary>Склад зберігання товарів.</summary>
public sealed class Warehouse
{
    /// <summary>Ідентифікатор складу.</summary>
    public int Id { get; init; }

    /// <summary>Назва складу.</summary>
    public string Name { get; init; } = string.Empty;

    /// <summary>Адреса складу.</summary>
    public string Address { get; init; } = string.Empty;
}
