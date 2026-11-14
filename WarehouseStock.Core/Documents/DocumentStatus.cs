namespace WarehouseStock.Core.Documents;

/// <summary>Стан документа переміщення товарів.</summary>
public enum DocumentStatus
{
    /// <summary>Новий документ.</summary>
    New,

    /// <summary>Переміщення погоджено.</summary>
    Approved,

    /// <summary>Товари відправлено.</summary>
    Dispatched,

    /// <summary>Переміщення скасовано.</summary>
    Cancelled,
}
