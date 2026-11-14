using WarehouseStock.Core.Legacy;

namespace WarehouseStock.Tests;

public sealed class LegacyProcessorTests
{
    [Fact]
    public void RegularRecipientReturns129270()
    {
        decimal total = LegacyProcessor.Process(
            1, "Центральний склад", "warehouse@example.com", true, SampleLines(),
            new DateOnly(2026, 3, 10), string.Empty, "New", false, 60m, out string status);

        Assert.Equal(1292.70m, total);
        Assert.Equal("Approved", status);
    }

    [Fact]
    public void SmallTransferAddsDelivery()
    {
        List<TransferLine> lines = new()
        {
            new TransferLine { Sku = "A1", Quantity = 2, UnitPrice = 200m },
        };
        decimal total = LegacyProcessor.Process(
            2, "Західний склад", "west@example.com", false, lines,
            new DateOnly(2026, 3, 10), "SALE10", "Approved", false, 60m, out string status);

        Assert.Equal(420m, total);
        Assert.Equal("Dispatched", status);
    }

    [Fact]
    public void NoLinesReturnsMinusOne()
    {
        decimal total = LegacyProcessor.Process(
            3, "Східний склад", "east@example.com", false, new List<TransferLine>(),
            new DateOnly(2026, 3, 10), string.Empty, "New", false, 60m, out _);

        Assert.Equal(-1m, total);
    }

    private static List<TransferLine> SampleLines() => new()
    {
        new TransferLine { Sku = "A1", Quantity = 12, UnitPrice = 100m },
        new TransferLine { Sku = "B2", Quantity = 1, UnitPrice = 250m },
    };
}
