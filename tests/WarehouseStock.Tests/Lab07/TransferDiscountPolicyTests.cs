using WarehouseStock.Core.Pricing;

namespace WarehouseStock.Tests.Lab07;

public sealed class TransferDiscountPolicyTests
{
    [Fact]
    public void DiscountFor_ZeroAmount_ReturnsZero()
    {
        // Arrange
        decimal amount = 0m;
        bool isRegular = false;

        // Act
        decimal actual = TransferDiscountPolicy.DiscountFor(amount, isRegular);

        // Assert
        Assert.Equal(0m, actual);
    }

    [Fact]
    public void DiscountFor_SmallRegular_ReturnsFivePercent()
    {
        // Arrange
        decimal amount = 400m;
        bool isRegular = true;

        // Act
        decimal actual = TransferDiscountPolicy.DiscountFor(amount, isRegular);

        // Assert
        Assert.Equal(20m, actual);
    }

    [Fact]
    public void DiscountFor_BelowBulkNew_ReturnsZero()
    {
        // Arrange
        decimal amount = 999.99m;
        bool isRegular = false;

        // Act
        decimal actual = TransferDiscountPolicy.DiscountFor(amount, isRegular);

        // Assert
        Assert.Equal(0m, actual);
    }

    [Fact]
    public void DiscountFor_BelowBulkRegular_ReturnsFifty()
    {
        // Arrange
        decimal amount = 999.99m;
        bool isRegular = true;

        // Act
        decimal actual = TransferDiscountPolicy.DiscountFor(amount, isRegular);

        // Assert
        Assert.Equal(50m, actual);
    }

    [Fact]
    public void DiscountFor_BulkBoundaryNew_ReturnsHundred()
    {
        // Arrange
        decimal amount = 1000m;
        bool isRegular = false;

        // Act
        decimal actual = TransferDiscountPolicy.DiscountFor(amount, isRegular);

        // Assert
        Assert.Equal(100m, actual);
    }

    [Fact]
    public void DiscountFor_BulkBoundaryRegular_ReturnsHundredFifty()
    {
        // Arrange
        decimal amount = 1000m;
        bool isRegular = true;

        // Act
        decimal actual = TransferDiscountPolicy.DiscountFor(amount, isRegular);

        // Assert
        Assert.Equal(150m, actual);
    }

}
