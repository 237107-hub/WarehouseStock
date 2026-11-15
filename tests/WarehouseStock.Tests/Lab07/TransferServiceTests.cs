using Microsoft.Extensions.Logging.Abstractions;
using WarehouseStock.Core.Domain;
using WarehouseStock.Core.Errors;
using WarehouseStock.Core.Services;

namespace WarehouseStock.Tests.Lab07;

public sealed class TransferServiceTests
{
    [Fact]
    public void TotalOf_TwoLines_SumsPricesFromStub()
    {
        // Arrange
        var repository = new FakeTransferRepository();
        var transfer = NewTransfer(7);
        transfer.AddLine(new TransferLine("A-1", 2, 10m));
        transfer.AddLine(new TransferLine("B-2", 1, 30m));
        var service = new TransferService(repository, new StubPricingPolicy(25m),
            new SpyNotifier(), NullLogger<TransferService>.Instance);

        // Act
        decimal actual = service.TotalOf(transfer);

        // Assert
        Assert.Equal(50m, actual);
    }

    [Fact]
    public void Place_ValidTransfer_StoresAndNotifiesOnce()
    {
        // Arrange
        var repository = new FakeTransferRepository();
        var notifier = new SpyNotifier();
        var service = new TransferService(repository, new StubPricingPolicy(1m),
            notifier, NullLogger<TransferService>.Instance);
        var transfer = NewTransfer(1);
        transfer.AddLine(new TransferLine("A-1", 1, 5m));

        // Act
        service.Place(transfer);

        // Assert
        Assert.Same(transfer, repository.GetById(1));
        Assert.Equal(1, repository.AddCalls);
        Assert.Equal(1, notifier.Calls);
        Assert.Same(transfer, notifier.LastTransfer);
    }

    [Fact]
    public void Load_UnknownId_ThrowsDomainRule()
    {
        // Arrange
        var service = new TransferService(new FakeTransferRepository(),
            new StubPricingPolicy(1m), new SpyNotifier(), NullLogger<TransferService>.Instance);

        // Act
        var error = Assert.Throws<DomainRuleException>(() => service.Load(99));

        // Assert
        Assert.Equal("transfer.exists", error.Rule);
    }

    private static Transfer NewTransfer(int id) => new(id, 10, 20, new DateOnly(2026, 11, 15));
}
