namespace WarehouseStock.Core.Documents;

/// <summary>Позиція успадкованого розрахунку переміщення.</summary>
public sealed class TransferLine
{
    /// <summary>Артикул товару.</summary>
    public string Sku { get; set; } = string.Empty;

    /// <summary>Кількість одиниць товару.</summary>
    public int Quantity { get; set; }

    /// <summary>Облікова ціна одиниці товару.</summary>
    public decimal UnitPrice { get; set; }
}
