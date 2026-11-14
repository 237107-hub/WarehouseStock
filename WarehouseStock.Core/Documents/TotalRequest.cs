namespace WarehouseStock.Core.Documents;

/// <summary>Параметри розрахунку без прапорця друку та вихідних аргументів.</summary>
public sealed record TotalRequest
{
    /// <summary>Номер документа.</summary>
    public int DocumentId { get; init; }

    /// <summary>Відомості про одержувача.</summary>
    public RecipientInfo Recipient { get; init; } = new(string.Empty, string.Empty, false);

    /// <summary>Позиції переміщення.</summary>
    public IReadOnlyList<TransferLine> Lines { get; init; } = Array.Empty<TransferLine>();

    /// <summary>Дата створення.</summary>
    public DateOnly CreatedAt { get; init; }

    /// <summary>Необов’язковий код купона.</summary>
    public string? CouponCode { get; init; }

    /// <summary>Початковий стан.</summary>
    public DocumentStatus Status { get; init; }

    /// <summary>Плата за доставку.</summary>
    public decimal DeliveryPrice { get; init; }
}
