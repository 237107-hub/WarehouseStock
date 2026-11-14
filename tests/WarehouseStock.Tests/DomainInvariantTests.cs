using WarehouseStock.Core.Domain;
using WarehouseStock.Core.Notifications;
using WarehouseStock.Core.Pricing;
using WarehouseStock.Core.Services;
using WarehouseStock.Core.Storage;

namespace WarehouseStock.Tests;

public sealed class DomainInvariantTests
{
    [Fact]
    public void TransferLineWithZeroQuantityThrowsArgumentOutOfRangeException()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new TransferLine("SKU-1", 0, 10m));
    }

    [Fact]
    public void TransferAfterApprovalRejectsNewLines()
    {
        Transfer transfer = NewTransfer(1);
        transfer.AddLine(new TransferLine("SKU-1", 1, 10m));
        transfer.Approve();

        Assert.Throws<InvalidOperationException>(() => transfer.AddLine(new TransferLine("SKU-2", 1, 5m)));
    }

    [Fact]
    public void TotalOfWithInjectedTenPercentDiscountReturns180()
    {
        InMemoryTransferRepository repository = new();
        TransferService service = new(repository, new DiscountPricingPolicy(0.10m),
            new TextWriterNotifier(TextWriter.Null));
        Transfer transfer = NewTransfer(2);
        transfer.AddLine(new TransferLine("SKU-1", 2, 100m));
        service.Place(transfer);

        Assert.Equal(180m, service.TotalOf(transfer));
        Assert.Same(transfer, repository.GetById(2));
    }

    private static Transfer NewTransfer(int id) => new(id, 10, 20, new DateOnly(2026, 11, 14));
}
