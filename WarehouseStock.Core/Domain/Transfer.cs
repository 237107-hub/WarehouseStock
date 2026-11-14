namespace WarehouseStock.Core.Domain;

/// <summary>Переміщення товарів між складами.</summary>
public sealed class Transfer
{
    private readonly List<TransferLine> _lines = new();

    public int Id { get; init; }

    public int SourceWarehouseId { get; init; }

    public int DestinationWarehouseId { get; init; }

    public TransferStatus Status { get; set; } = TransferStatus.New;

    public DateTimeOffset CreatedAt { get; init; }

    public IReadOnlyList<TransferLine> Lines => _lines;

    /// <summary>Додає позицію до документа.</summary>
    /// <param name="line">Позиція товару для переміщення.</param>
    public void AddLine(TransferLine line) => _lines.Add(line);

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
