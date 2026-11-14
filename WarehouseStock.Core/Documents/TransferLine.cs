namespace WarehouseStock.Core.Documents;

/// <summary>Позиція успадкованого розрахунку переміщення.</summary>
public sealed record TransferLine
{
    /// <summary>Артикул товару.</summary>
    public string Sku { get; init; } = string.Empty;

    /// <summary>Кількість одиниць товару.</summary>
    public int Quantity { get; init; }

    /// <summary>Облікова ціна одиниці товару.</summary>
    public decimal UnitPrice { get; init; }
}
