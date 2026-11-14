namespace WarehouseStock.Core.Domain;

/// <summary>Стан документа переміщення.</summary>
public enum TransferStatus
{
    /// <summary>Документ створено.</summary>
    New = 0,

    /// <summary>Переміщення погоджено.</summary>
    Approved = 1,

    /// <summary>Товари перебувають у дорозі.</summary>
    InTransit = 2,

    /// <summary>Переміщення завершено.</summary>
    Completed = 3,

    /// <summary>Переміщення скасовано.</summary>
    Cancelled = 4,
}
