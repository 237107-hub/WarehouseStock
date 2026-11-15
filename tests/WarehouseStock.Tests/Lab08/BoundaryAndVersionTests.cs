using System.Globalization;
using WarehouseStock.Core;
using WarehouseStock.Core.Input;

namespace WarehouseStock.Tests.Lab08;

public sealed class BoundaryAndVersionTests
{
    private static readonly string[] _reportRows = { "A", "B" };

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("SKU-1;2")]
    [InlineData("SKU-1;2;10.50;extra")]
    [InlineData(";2;10.50")]
    [InlineData("123456789012345678901;2;10.50")]
    [InlineData("SKU-1;0;10.50")]
    [InlineData("SKU-1;-2;10.50")]
    [InlineData("SKU-1;1001;10.50")]
    [InlineData("SKU-1;2147483648;10.50")]
    [InlineData("SKU-1;2;-10.50")]
    [InlineData("SKU-1;2;10,50")]
    [InlineData("SKU-1;2;1,000")]
    [InlineData("SKU-1;2;1e2")]
    [InlineData("SKU-1;2;1000000000.01")]
    public void TryParse_InvalidInput_ReturnsFalseAndNull(string? raw)
    {
        bool ok = TransferCsvLineParser.TryParse(raw, out var line);

        Assert.False(ok);
        Assert.Null(line);
    }

    [Theory]
    [InlineData("SKU-1;1;0", 1, 0)]
    [InlineData(" SKU-1 ;2;10.50", 2, 21)]
    [InlineData("12345678901234567890;1000;1000000000", 1000, 1000000000000)]
    public void TryParse_ValidBoundaries_CreatesSafeLine(string raw, int quantity, double expected)
    {
        bool ok = TransferCsvLineParser.TryParse(raw, out var line);

        Assert.True(ok);
        Assert.NotNull(line);
        Assert.Equal(quantity, line.Quantity);
        Assert.Equal((decimal)expected, line.Amount);
        Assert.Equal(line.Sku.Trim(), line.Sku);
    }

    [Fact]
    public void TryParse_LongRawLine_ReturnsFalse()
    {
        bool ok = TransferCsvLineParser.TryParse(new string('A', 257), out var line);

        Assert.False(ok);
        Assert.Null(line);
    }

    [Fact]
    public void TryParse_CommaCulture_StillRequiresDecimalPoint()
    {
        CultureInfo previous = CultureInfo.CurrentCulture;
        try
        {
            CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("uk-UA");
            bool ok = TransferCsvLineParser.TryParse("A;2;10.50", out var line);

            Assert.True(ok);
            Assert.NotNull(line);
            Assert.Equal(21m, line.Amount);
        }
        finally
        {
            CultureInfo.CurrentCulture = previous;
        }
    }

    [Fact]
    public void Current_ConfiguredAssembly_ReturnsVersionPrefix()
    {
        string actual = VersionInfo.Current();

        Assert.Equal("0.8.0", actual);
    }

    [Fact]
    public void Build_TwoRows_ReturnsUppercaseHeaderAndRows()
    {
        var builder = new ReportBuilder { Title = "Report" };

        string actual = builder.Build(_reportRows);

        Assert.Equal("REPORT" + Environment.NewLine + "A" + Environment.NewLine +
            "B" + Environment.NewLine, actual);
    }

    [Fact]
    public void Build_NullRows_ThrowsForRows()
    {
        var builder = new ReportBuilder();

        var error = Assert.Throws<ArgumentNullException>(() => builder.Build(null!));

        Assert.Equal("rows", error.ParamName);
    }
}
