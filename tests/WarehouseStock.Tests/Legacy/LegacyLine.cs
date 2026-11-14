namespace WarehouseStock.Tests.Legacy;

// Навмисно слабкий контракт із методички; тільки навчальний еталон аудиту.
public sealed class LegacyLine
{
    public string Sku { get; set; } = string.Empty;

    public int Quantity { get; set; }

    public decimal UnitPrice { get; set; }
}
