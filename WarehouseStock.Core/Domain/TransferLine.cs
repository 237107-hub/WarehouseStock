namespace WarehouseStock.Core.Domain;

/// <summary>Позиція документа переміщення товару.</summary>
public sealed class TransferLine
{
    public string Sku { get; init; } = string.Empty;

    public int Quantity { get; init; }

    public decimal UnitPrice { get; init; }

    public decimal Amount => Quantity * UnitPrice;
}
