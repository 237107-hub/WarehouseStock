using WarehouseStock.Core.Documents;

namespace WarehouseStock.Tests;

public sealed class PricingRuleTests
{
    [Theory]
    [InlineData(9, 900)]
    [InlineData(10, 950)]
    public void BulkThresholdIsInclusive(int quantity, int expected)
    {
        TransferLine[] lines = { new() { Sku = "A1", Quantity = quantity, UnitPrice = 100m } };
        Assert.Equal(expected, DocumentTotalCalculator.SubtotalOf(lines));
    }

    [Theory]
    [InlineData(2000, 0.0)]
    [InlineData(2001, 100.05)]
    [InlineData(5000, 250.0)]
    [InlineData(5001, 500.1)]
    public void VolumeThresholdsAreStrict(int amount, double expected)
    {
        Assert.Equal((decimal)expected, DocumentTotalCalculator.VolumeDiscountOf(amount));
    }

    [Theory]
    [InlineData(1000, "MINUS200", 0)]
    [InlineData(1001, "MINUS200", 200)]
    [InlineData(1000, "SALE10", 100)]
    [InlineData(1000, "sale10", 0)]
    [InlineData(1000, "UNKNOWN", 0)]
    [InlineData(1000, null, 0)]
    public void CouponContractPreservesCaseAndThreshold(int amount, string? code, int expected)
    {
        Assert.Equal(expected, DocumentTotalCalculator.CouponDiscountOf(amount, code));
    }

    [Theory]
    [InlineData(999, 60)]
    [InlineData(1000, 0)]
    public void DeliveryThresholdIsInclusive(int amount, int expected)
    {
        Assert.Equal(expected, DocumentTotalCalculator.DeliveryFeeOf(amount, 60m));
    }

    [Theory]
    [InlineData(DocumentStatus.New, 0, DocumentStatus.New)]
    [InlineData(DocumentStatus.New, 1, DocumentStatus.Approved)]
    [InlineData(DocumentStatus.Approved, 0, DocumentStatus.Dispatched)]
    [InlineData(DocumentStatus.Dispatched, 1, DocumentStatus.Dispatched)]
    [InlineData(DocumentStatus.Cancelled, 1, DocumentStatus.Cancelled)]
    public void StateTransitionsAreExplicit(DocumentStatus current, int amount, DocumentStatus expected)
    {
        Assert.Equal(expected, DocumentTotalCalculator.NextStatusOf(current, amount));
    }

    [Theory]
    [InlineData(8, 20)]
    [InlineData(10, 0)]
    public void SundayDiscountUsesDocumentDate(int day, int expected)
    {
        Assert.Equal(expected, DocumentTotalCalculator.WeekendDiscountOf(1000m, new DateOnly(2026, 3, day)));
    }

    [Fact]
    public void CancelledDocumentReturnsZeroWithoutChangingRequest()
    {
        TotalRequest request = TestData.Request(status: DocumentStatus.Cancelled);
        TotalResult first = DocumentTotalCalculator.Calculate(request);
        TotalResult second = DocumentTotalCalculator.Calculate(request);
        Assert.Equal(0m, first.Total);
        Assert.Equal(DocumentStatus.Cancelled, first.NewStatus);
        Assert.Equal(first, second);
        Assert.Equal(400m, request.Lines[0].UnitPrice);
    }

    [Theory]
    [InlineData("client")]
    [InlineData(" ")]
    public void InvalidEmailIsRejected(string email)
    {
        TotalRequest request = TestData.Request() with { Recipient = new RecipientInfo("Склад", email, false) };
        Assert.Throws<ArgumentException>(() => DocumentTotalCalculator.Calculate(request));
    }

    [Fact]
    public void NullInputsAreRejected()
    {
        Assert.Throws<ArgumentNullException>(() => DocumentTotalCalculator.Calculate(null!));
        Assert.Throws<ArgumentNullException>(() => DocumentTotalCalculator.SubtotalOf(null!));
        Assert.Throws<ArgumentNullException>(() => DocumentGuards.EnsureLineValid(null!));
        Assert.Throws<ArgumentNullException>(() => DocumentTotalCalculator.Calculate(TestData.Request() with { Recipient = null! }));
    }

    [Fact]
    public void InvalidHeaderStatusAndDeliveryAreRejected()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => DocumentTotalCalculator.Calculate(TestData.Request() with { DocumentId = 0 }));
        Assert.Throws<ArgumentOutOfRangeException>(() => DocumentTotalCalculator.Calculate(TestData.Request() with { Status = (DocumentStatus)99 }));
        Assert.Throws<ArgumentOutOfRangeException>(() => DocumentTotalCalculator.Calculate(TestData.Request() with { DeliveryPrice = -1m }));
    }

    [Fact]
    public void InvalidLineValuesAreRejected()
    {
        Assert.Throws<ArgumentException>(() => DocumentGuards.EnsureLineValid(new TransferLine { Sku = " ", Quantity = 1 }));
        Assert.Throws<ArgumentOutOfRangeException>(() => DocumentGuards.EnsureLineValid(new TransferLine { Sku = "A", Quantity = 1, UnitPrice = -1m }));
    }

    [Fact]
    public void EveryAmountRuleRejectsNegativeInput()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => DocumentTotalCalculator.ClientDiscountOf(-1m, false));
        Assert.Throws<ArgumentOutOfRangeException>(() => DocumentTotalCalculator.VolumeDiscountOf(-1m));
        Assert.Throws<ArgumentOutOfRangeException>(() => DocumentTotalCalculator.CouponDiscountOf(-1m, null));
        Assert.Throws<ArgumentOutOfRangeException>(() => DocumentTotalCalculator.WeekendDiscountOf(-1m, default));
        Assert.Throws<ArgumentOutOfRangeException>(() => DocumentTotalCalculator.DeliveryFeeOf(-1m, 0m));
        Assert.Throws<ArgumentOutOfRangeException>(() => DocumentTotalCalculator.DeliveryFeeOf(0m, -1m));
        Assert.Throws<ArgumentOutOfRangeException>(() => DocumentTotalCalculator.NextStatusOf(DocumentStatus.New, -1m));
    }
}
