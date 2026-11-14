using System.Collections.ObjectModel;

namespace WarehouseStock.Core.Domain;

/// <summary>Агрегат переміщення з контрольованим життєвим циклом.</summary>
public sealed class Transfer
{
    private readonly List<TransferLine> _lines = new();
    private readonly ReadOnlyCollection<TransferLine> _view;

    /// <summary>Створює новий документ між різними складами.</summary>
    /// <param name="id">Додатний номер документа.</param>
    /// <param name="sourceWarehouseId">Ідентифікатор відправника.</param>
    /// <param name="destinationWarehouseId">Ідентифікатор одержувача.</param>
    /// <param name="createdAt">Явно передана дата створення.</param>
    public Transfer(int id, int sourceWarehouseId, int destinationWarehouseId, DateOnly createdAt)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(id);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(sourceWarehouseId);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(destinationWarehouseId);
        if (sourceWarehouseId == destinationWarehouseId)
        {
            throw new ArgumentException("Склади мають відрізнятися.", nameof(destinationWarehouseId));
        }

        Id = id;
        SourceWarehouseId = sourceWarehouseId;
        DestinationWarehouseId = destinationWarehouseId;
        CreatedAt = createdAt;
        _view = _lines.AsReadOnly();
    }

    /// <summary>Номер документа.</summary>
    public int Id { get; }

    /// <summary>Склад відправлення.</summary>
    public int SourceWarehouseId { get; }

    /// <summary>Склад призначення.</summary>
    public int DestinationWarehouseId { get; }

    /// <summary>Дата створення документа.</summary>
    public DateOnly CreatedAt { get; }

    /// <summary>Стан, змінюваний лише операціями агрегату.</summary>
    public TransferStatus Status { get; private set; } = TransferStatus.New;

    /// <summary>Захищене від запису подання позицій.</summary>
    public IReadOnlyList<TransferLine> Lines => _view;

    /// <summary>Додає позицію тільки до нового документа.</summary>
    /// <param name="line">Незмінна перевірена позиція.</param>
    public void AddLine(TransferLine line)
    {
        ArgumentNullException.ThrowIfNull(line);
        EnsureStatus(TransferStatus.New);
        _lines.Add(line);
    }

    /// <summary>Погоджує непорожній новий документ.</summary>
    public void Approve()
    {
        EnsureStatus(TransferStatus.New);
        if (_lines.Count == 0)
        {
            throw new InvalidOperationException("Порожнє переміщення погодити не можна.");
        }

        Status = TransferStatus.Approved;
    }

    /// <summary>Відправляє погоджене переміщення.</summary>
    public void Dispatch()
    {
        EnsureStatus(TransferStatus.Approved);
        Status = TransferStatus.InTransit;
    }

    /// <summary>Завершує переміщення, яке перебуває в дорозі.</summary>
    public void Complete()
    {
        EnsureStatus(TransferStatus.InTransit);
        Status = TransferStatus.Completed;
    }

    /// <summary>Скасовує новий або погоджений документ.</summary>
    public void Cancel()
    {
        if (Status is not (TransferStatus.New or TransferStatus.Approved))
        {
            throw new InvalidOperationException("Переміщення вже відправлене або закрите.");
        }

        Status = TransferStatus.Cancelled;
    }

    private void EnsureStatus(TransferStatus expected)
    {
        if (Status != expected)
        {
            throw new InvalidOperationException("Перехід із поточного стану заборонено.");
        }
    }
}
