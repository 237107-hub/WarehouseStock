namespace WarehouseStock.Core.Domain;

/// <summary>Позиція документа переміщення товару.</summary>
public sealed class TransferLine
{
    /// <summary>Артикул переміщуваного товару.</summary>
    public string Sku { get; init; } = string.Empty;

    /// <summary>Кількість одиниць товару.</summary>
    public int Quantity { get; init; }

    /// <summary>Облікова ціна одиниці товару.</summary>
    public decimal UnitPrice { get; init; }

    /// <summary>Вартість позиції як добуток кількості та ціни.</summary>
    public decimal Amount => Quantity * UnitPrice;
}
