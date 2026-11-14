using WarehouseStock.Core.Documents;

namespace WarehouseStock.Tests;

public sealed class CalculatorTests
{
    [Fact]
    public void RegularRecipientReturns129270()
    {
        TotalRequest request = new()
        {
            DocumentId = 1,
            Recipient = new RecipientInfo("Центральний склад", "warehouse@example.com", true),
            Lines = new[]
            {
                new TransferLine { Sku = "A1", Quantity = 12, UnitPrice = 100m },
                new TransferLine { Sku = "B2", Quantity = 1, UnitPrice = 250m },
            },
            CreatedAt = new DateOnly(2026, 3, 10),
            DeliveryPrice = 60m,
        };

        TotalResult result = DocumentTotalCalculator.Calculate(request);

        Assert.Equal(1292.70m, result.Total);
        Assert.Equal(DocumentStatus.Approved, result.NewStatus);
        Assert.Equal(DocumentStatus.New, request.Status);
    }

    [Fact]
    public void SmallTransferAddsDelivery()
    {
        TotalRequest request = TestData.Request(400m, "SALE10", DocumentStatus.Approved);

        TotalResult result = DocumentTotalCalculator.Calculate(request);

        Assert.Equal(420m, result.Total);
        Assert.Equal(DocumentStatus.Dispatched, result.NewStatus);
    }

    [Fact]
    public void NoLinesThrowArgumentException()
    {
        TotalRequest request = TestData.Request() with { Lines = Array.Empty<TransferLine>() };

        Assert.Throws<ArgumentException>(() => DocumentTotalCalculator.Calculate(request));
    }
}
