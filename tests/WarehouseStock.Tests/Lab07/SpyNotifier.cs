using WarehouseStock.Core.Abstractions;
using WarehouseStock.Core.Domain;

namespace WarehouseStock.Tests.Lab07;

internal sealed class SpyNotifier : INotifier
{
    public int Calls { get; private set; }

    public Transfer? LastTransfer { get; private set; }

    public void Notify(Transfer transfer)
    {
        Calls++;
        LastTransfer = transfer;
    }
}
