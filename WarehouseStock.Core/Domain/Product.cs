namespace WarehouseStock.Core.Domain;

/// <summary>Товар складського обліку.</summary>
public sealed class Product
{
    /// <summary>Артикул товару.</summary>
    public string Sku { get; init; } = string.Empty;

    /// <summary>Назва товару.</summary>
    public string Title { get; init; } = string.Empty;

    /// <summary>Облікова ціна одиниці товару.</summary>
    public decimal Price { get; init; }

    /// <summary>Залишок товару в цілих одиницях.</summary>
    public int Stock { get; init; }
}
