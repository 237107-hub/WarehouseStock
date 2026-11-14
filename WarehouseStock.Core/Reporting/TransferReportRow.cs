using WarehouseStock.Core.Domain;

namespace WarehouseStock.Core.Reporting;

/// <summary>Незмінний знімок трьох значень для CSV без залежності від сервісу.</summary>
public sealed record TransferReportRow
{
    /// <summary>Створює перевірений рядок звіту.</summary>
    /// <param name="id">Номер переміщення.</param>
    /// <param name="total">Невід’ємна облікова вартість.</param>
    /// <param name="status">Визначений стан документа.</param>
    public TransferReportRow(int id, decimal total, TransferStatus status)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(id);
        ArgumentOutOfRangeException.ThrowIfNegative(total);
        if (!Enum.IsDefined(status))
        {
            throw new ArgumentOutOfRangeException(nameof(status));
        }

        Id = id;
        Total = total;
        Status = status;
    }

    /// <summary>Номер документа.</summary>
    public int Id { get; }

    /// <summary>Облікова вартість на момент знімка.</summary>
    public decimal Total { get; }

    /// <summary>Стан на момент знімка.</summary>
    public TransferStatus Status { get; }
}
