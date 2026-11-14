namespace WarehouseStock.Core.Domain;

/// <summary>Незмінна позиція переміщення з перевіреним інваріантом.</summary>
public sealed record TransferLine
{
    /// <summary>Створює позицію з обліковою ціною на дату документа.</summary>
    /// <param name="sku">Непорожній артикул товару.</param>
    /// <param name="quantity">Додатна кількість одиниць.</param>
    /// <param name="unitPrice">Невід’ємна ціна одиниці.</param>
    public TransferLine(string sku, int quantity, decimal unitPrice)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(sku);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(quantity);
        ArgumentOutOfRangeException.ThrowIfNegative(unitPrice);
        Sku = sku;
        Quantity = quantity;
        UnitPrice = unitPrice;
    }

    /// <summary>Артикул переміщуваного товару.</summary>
    public string Sku { get; }

    /// <summary>Кількість одиниць товару.</summary>
    public int Quantity { get; }

    /// <summary>Зафіксована ціна одиниці товару.</summary>
    public decimal UnitPrice { get; }

    /// <summary>Вартість позиції як добуток кількості та ціни.</summary>
    public decimal Amount => Quantity * UnitPrice;
}
