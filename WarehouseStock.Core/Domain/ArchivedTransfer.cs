namespace WarehouseStock.Core.Domain;

/// <summary>Архівне подання термінального документа через композицію.</summary>
public sealed class ArchivedTransfer
{
    private readonly Transfer _transfer;

    /// <summary>Закріплює подання документа, який більше не можна змінити.</summary>
    /// <param name="transfer">Завершене або скасоване переміщення.</param>
    /// <param name="archivedAt">Дата архівування не раніше створення.</param>
    public ArchivedTransfer(Transfer transfer, DateOnly archivedAt)
    {
        ArgumentNullException.ThrowIfNull(transfer);
        if (transfer.Status is not (TransferStatus.Completed or TransferStatus.Cancelled))
        {
            throw new InvalidOperationException("Архівують лише закриті документи.");
        }

        ArgumentOutOfRangeException.ThrowIfLessThan(archivedAt, transfer.CreatedAt);
        _transfer = transfer;
        ArchivedAt = archivedAt;
    }

    /// <summary>Дата архівування документа.</summary>
    public DateOnly ArchivedAt { get; }

    /// <summary>Номер вкладеного документа.</summary>
    public int Id => _transfer.Id;

    /// <summary>Термінальний стан вкладеного документа.</summary>
    public TransferStatus Status => _transfer.Status;

    /// <summary>Позиції, недоступні для зовнішнього запису.</summary>
    public IReadOnlyList<TransferLine> Lines => _transfer.Lines;
}
