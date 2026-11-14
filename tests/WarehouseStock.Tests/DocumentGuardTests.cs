using WarehouseStock.Core.Documents;

namespace WarehouseStock.Tests;

public sealed class DocumentGuardTests
{
    [Fact]
    public void EmptyLinesThrowArgumentException()
    {
        Assert.Throws<ArgumentException>(() =>
            DocumentGuards.EnsureLinesValid(Array.Empty<TransferLine>()));
    }

    [Fact]
    public void ZeroQuantityThrowsRangeException()
    {
        TransferLine line = new() { Sku = "A1", Quantity = 0, UnitPrice = 10m };

        ArgumentOutOfRangeException exception = Assert.Throws<ArgumentOutOfRangeException>(() =>
            DocumentGuards.EnsureLineValid(line));

        Assert.Equal("Quantity", exception.ParamName);
    }
}
