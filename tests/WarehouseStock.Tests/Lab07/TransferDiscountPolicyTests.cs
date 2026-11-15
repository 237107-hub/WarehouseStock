using WarehouseStock.Core.Pricing;

namespace WarehouseStock.Tests.Lab07;

public sealed class TransferDiscountPolicyTests
{
    [Theory]
    [InlineData(0, false, 0)]
    [InlineData(400, true, 20)]
    [InlineData(999.99, false, 0)]
    [InlineData(999.99, true, 50)]
    [InlineData(1000, false, 100)]
    [InlineData(1000, true, 150)]
    [InlineData(1000.01, false, 100)]
    [InlineData(0.1, true, 0)]
    [InlineData(0.3, true, 0.02)]
    public void DiscountFor_Boundaries_MatchesTable(double amount, bool isRegular, double expected)
    {
        // Arrange
        decimal value = (decimal)amount;

        // Act
        decimal actual = TransferDiscountPolicy.DiscountFor(value, isRegular);

        // Assert
        Assert.Equal((decimal)expected, actual);
    }

    [Theory]
    [InlineData(-0.01)]
    [InlineData(-1000)]
    public void DiscountFor_NegativeAmount_ThrowsOutOfRange(double amount)
    {
        // Arrange
        decimal value = (decimal)amount;

        // Act
        var error = Assert.Throws<ArgumentOutOfRangeException>(
            () => TransferDiscountPolicy.DiscountFor(value, true));

        // Assert
        Assert.Equal("amount", error.ParamName);
    }
}
