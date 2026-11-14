namespace WarehouseStock.Core.Domain;

/// <summary>Переміщення товарів між складами.</summary>
public sealed class Transfer
{
    private readonly List<TransferLine> _lines = new();

    /// <summary>Ідентифікатор документа переміщення.</summary>
    public int Id { get; init; }

    /// <summary>Ідентифікатор складу відправлення.</summary>
    public int SourceWarehouseId { get; init; }

    /// <summary>Ідентифікатор складу призначення.</summary>
    public int DestinationWarehouseId { get; init; }

    /// <summary>Поточний стан переміщення.</summary>
    public TransferStatus Status { get; set; } = TransferStatus.New;

    /// <summary>Час створення документа з часовим зміщенням.</summary>
    public DateTimeOffset CreatedAt { get; init; }

    /// <summary>Позиції документа через інтерфейс читання.</summary>
    public IReadOnlyList<TransferLine> Lines => _lines;

    /// <summary>Додає позицію до документа.</summary>
    /// <param name="line">Позиція товару для переміщення.</param>
    public void AddLine(TransferLine line)
    {
        ArgumentNullException.ThrowIfNull(line);
        ArgumentException.ThrowIfNullOrWhiteSpace(line.Sku);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(line.Quantity, nameof(line.Quantity));
        ArgumentOutOfRangeException.ThrowIfNegative(line.UnitPrice, nameof(line.UnitPrice));
        _lines.Add(line);
    }

    /// <summary>Обчислює загальну вартість переміщення.</summary>
    /// <returns>Сума вартостей усіх позицій документа.</returns>
    public decimal Total()
    {
        decimal sum = 0m;
        foreach (TransferLine line in _lines)
        {
            sum += line.Amount;
        }

        return sum;
    }
}
