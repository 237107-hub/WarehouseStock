using WarehouseStock.Core.Abstractions;
using WarehouseStock.Core.Domain;

namespace WarehouseStock.Core.Storage;

/// <summary>Сховище в пам’яті без файлів і мережевих залежностей.</summary>
public sealed class InMemoryTransferRepository : ITransferRepository
{
    private readonly Dictionary<int, Transfer> _transfers = new();

    /// <inheritdoc/>
    public void Add(Transfer transfer)
    {
        ArgumentNullException.ThrowIfNull(transfer);
        if (!_transfers.TryAdd(transfer.Id, transfer))
        {
            throw new InvalidOperationException("Номер документа вже зареєстрований.");
        }
    }

    /// <inheritdoc/>
    public Transfer? GetById(int id)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(id);
        return _transfers.GetValueOrDefault(id);
    }

    /// <inheritdoc/>
    public IReadOnlyList<Transfer> GetAll() => _transfers.Values.ToArray();
}
