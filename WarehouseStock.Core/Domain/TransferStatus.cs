namespace WarehouseStock.Core.Domain;

/// <summary>Стан документа переміщення.</summary>
public enum TransferStatus
{
    New = 0,
    Approved = 1,
    InTransit = 2,
    Completed = 3,
    Cancelled = 4,
}
