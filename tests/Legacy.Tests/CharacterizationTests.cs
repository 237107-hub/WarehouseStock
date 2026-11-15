using System.Globalization;
using WarehouseStock.Legacy;
using Xunit;

namespace WarehouseStock.Legacy.Tests;

public sealed class CharacterizationTests : IDisposable
{
    private readonly CultureInfo previous = CultureInfo.CurrentCulture;
    private readonly LegacyTransferProcessor processor = new();

    public CharacterizationTests() => CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;

    public void Dispose() => CultureInfo.CurrentCulture = previous;

    private static List<TransferItem> Items() => new()
    {
        new() { Code = "A-1", Qty = 2, Price = 150m },
        new() { Code = "B-2", Qty = 1, Price = 400m },
    };

    [Fact]
    public void NullItemsWinOverOtherErrors() => Assert.Equal("ERR: null",
        TestApi.Handle(processor, null, "bad", null));

    [Fact]
    public void EmptyItemsWinOverStateAndMail() => Assert.Equal("ERR: empty",
        TestApi.Handle(processor, new(), "bad", null));

    [Fact]
    public void StateWinsOverMail() => Assert.Equal("ERR: state",
        TestApi.Handle(processor, Items(), "NEW", null));

    [Theory]
    [InlineData(null)]
    [InlineData("invalid")]
    public void InvalidMail(string? mail) => Assert.Equal("ERR: mail",
        TestApi.Handle(processor, Items(), mail: mail));

    [Fact]
    public void GoldenReport()
    {
        Assert.Equal("Документ #1001\nКлієнт: Іван\nA-1 x2 = 300.00 UAH\n" +
            "B-2 x1 = 400.00 UAH\nЗнижка: 105.00 UAH\nДоставка: 60.00 UAH\nРазом: 655.00 UAH\n",
            TestApi.Handle(processor, Items()));
    }

    [Theory]
    [InlineData("regular", 0, "760")]
    [InlineData("vip", 0, "655")]
    [InlineData("staff", 0, "550")]
    [InlineData("regular", 11, "725")]
    [InlineData("regular", 10, "760")]
    [InlineData("VIP", 0, "760")]
    [InlineData("unknown", 11, "725")]
    [InlineData("vip", 100, "655")]
    [InlineData("staff", 100, "550")]
    public void RatesAndPriority(string kind, int done, string expected) =>
        Assert.Equal(decimal.Parse(expected, CultureInfo.InvariantCulture),
            TestApi.Preview(processor, kind, done, Items()));

    [Theory]
    [InlineData("vip")]
    [InlineData("staff")]
    [InlineData("regular")]
    public void DiscountCap(string kind) => Assert.Equal(19500m,
        TestApi.Preview(processor, kind, 11, new() { new() { Qty = 1, Price = 20000m } }));

    [Theory]
    [InlineData("1000", "1000")]
    [InlineData("999.99", "1059.99")]
    [InlineData("0", "60")]
    [InlineData("-100", "-40")]
    public void ShippingBoundaryAndNegative(string price, string expected) =>
        Assert.Equal(decimal.Parse(expected, CultureInfo.InvariantCulture),
            TestApi.Preview(processor, "regular", 0, new()
            { new() { Qty = 1, Price = decimal.Parse(price, CultureInfo.InvariantCulture) } }));

    [Fact]
    public void NegativeDiscountIsPreserved() => Assert.Equal(-25m,
        TestApi.Preview(processor, "vip", 0, new() { new() { Qty = -1, Price = 100m } }));

    [Fact]
    public void EmptyPreviewStillChargesDelivery() => Assert.Equal(60m,
        TestApi.Preview(processor, "regular", 0, new()));

    [Fact]
    public void NullPreviewThrows() => Assert.Throws<NullReferenceException>(() =>
        TestApi.Preview(processor, "regular", 0, null!));

    [Fact]
    public void JournalOrderAndRejectedCall()
    {
        Assert.Equal("", processor.DumpLog());
        TestApi.Handle(processor, Items(), send: true);
        TestApi.Handle(processor, null);
        TestApi.Handle(processor, Items(), state: "paid");
        Assert.Equal("ok 1001\nmail -> a@b\nok 1001\n", processor.DumpLog());
    }

    [Fact]
    public void DescribeUsesClockAndCulture() => Assert.Equal(
        "ІВАН [VIP] [ЛОЯЛЬНИЙ] <a@b> стаж " + (DateTime.Now.Year - 2020),
        TestApi.Describe(processor, " Іван ", "vip", 11, "A@B"));

    [Fact]
    public void DescribeBoundary() => Assert.Equal("USER <a@b> стаж " + (DateTime.Now.Year - 2020),
        TestApi.Describe(processor, " user ", "staff", 10, "A@B"));

    [Fact]
    public void CommaDecimalCultureIsPreserved()
    {
        CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("uk-UA");
        Assert.Contains("Разом: 655,00 UAH\n", TestApi.Handle(processor, Items()));
    }

    [Fact]
    public void TurkishCaseIsPreserved()
    {
        CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("tr-TR");
        Assert.StartsWith("İ <ı@b>", TestApi.Describe(processor, "i", "regular", 0, "I@B"));
    }
}
