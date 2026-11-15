using WarehouseStock.Core.Abstractions;
using WarehouseStock.Core.Domain;

namespace WarehouseStock.Tests.Lab07;

internal sealed class FakeTransferRepository : ITransferRepository
{
    private readonly Dictionary<int, Transfer> _items = new();

    public int AddCalls { get; private set; }

    public void Add(Transfer transfer)
    {
        _items.Add(transfer.Id, transfer);
        AddCalls++;
    }

    public Transfer? GetById(int id) => _items.GetValueOrDefault(id);

    public IReadOnlyList<Transfer> GetAll() => _items.Values.ToArray();
}
