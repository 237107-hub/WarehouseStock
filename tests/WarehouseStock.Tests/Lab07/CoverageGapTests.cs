using WarehouseStock.Core.Domain;
using WarehouseStock.Core.Reporting;

namespace WarehouseStock.Tests.Lab07;

public sealed class CoverageGapTests
{
    [Fact]
    public void Warehouse_MailWithoutAt_ThrowsForEmail()
    {
        // Arrange
        const string email = "invalid";

        // Act
        var error = Assert.Throws<ArgumentException>(() => new Warehouse(1, "Central", email));

        // Assert
        Assert.Equal("email", error.ParamName);
    }

    [Fact]
    public void Receive_PositiveQuantity_IncreasesStock()
    {
        // Arrange
        var product = new Product("A", "Box", 10m, 2);

        // Act
        product.Receive(3);

        // Assert
        Assert.Equal(5, product.Stock);
    }

    [Fact]
    public void Dispatch_ExactStock_LeavesZero()
    {
        // Arrange
        var product = new Product("A", "Box", 10m, 2);

        // Act
        product.Dispatch(2);

        // Assert
        Assert.Equal(0, product.Stock);
    }

    [Fact]
    public void Dispatch_InsufficientStock_ThrowsWithoutMutation()
    {
        // Arrange
        var product = new Product("A", "Box", 10m, 2);

        // Act
        Assert.Throws<InvalidOperationException>(() => product.Dispatch(3));

        // Assert
        Assert.Equal(2, product.Stock);
    }

    [Fact]
    public void Archive_OpenTransfer_Throws()
    {
        // Arrange
        var transfer = NewTransfer();

        // Act
        var error = Assert.Throws<InvalidOperationException>(
            () => new ArchivedTransfer(transfer, new DateOnly(2026, 11, 15)));

        // Assert
        Assert.Equal("Архівують лише закриті документи.", error.Message);
    }

    [Theory]
    [InlineData(false, TransferStatus.Cancelled)]
    [InlineData(true, TransferStatus.Completed)]
    public void Archive_TerminalTransfer_PreservesSnapshot(bool complete, TransferStatus expected)
    {
        // Arrange
        var transfer = NewTransfer();
        transfer.AddLine(new TransferLine("A", 1, 10m));
        if (complete)
        {
            transfer.Approve();
            transfer.Dispatch();
            transfer.Complete();
        }
        else
        {
            transfer.Cancel();
        }

        // Act
        var archive = new ArchivedTransfer(transfer, new DateOnly(2026, 11, 15));

        // Assert
        Assert.Equal(expected, archive.Status);
        Assert.Equal(transfer.Id, archive.Id);
        Assert.Single(archive.Lines);
        Assert.Equal(new DateOnly(2026, 11, 15), archive.ArchivedAt);
    }

    [Fact]
    public void BuildCsv_OneRow_ReturnsExactInvariantText()
    {
        // Arrange
        TransferReportRow[] rows = { new(7, 12.5m, TransferStatus.New) };

        // Act
        string actual = TransferCsvReport.BuildCsv(rows);

        // Assert
        Assert.Equal("id;total;status\n7;12.50;New\n", actual);
    }

    [Fact]
    public void BuildCsv_EmptyRows_ReturnsHeader()
    {
        // Arrange
        TransferReportRow[] rows = Array.Empty<TransferReportRow>();

        // Act
        string actual = TransferCsvReport.BuildCsv(rows);

        // Assert
        Assert.Equal("id;total;status\n", actual);
    }

    [Fact]
    public void ReportRow_UndefinedStatus_ThrowsForStatus()
    {
        // Arrange
        const TransferStatus invalid = (TransferStatus)999;

        // Act
        var error = Assert.Throws<ArgumentOutOfRangeException>(
            () => new TransferReportRow(7, 12.5m, invalid));

        // Assert
        Assert.Equal("status", error.ParamName);
    }

    private static Transfer NewTransfer() => new(1, 10, 20, new DateOnly(2026, 11, 15));
}
