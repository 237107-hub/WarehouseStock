using System.Globalization;
using WarehouseStock.Core.Documents;
using WarehouseStock.Core.Reporting;

namespace WarehouseStock.Tests;

public sealed class ReportTests
{
    [Fact]
    public void ReportIsIdenticalAcrossCultures()
    {
        CultureInfo previous = CultureInfo.CurrentCulture;
        try
        {
            TotalRequest request = TestData.Request(coupon: "SALE10", status: DocumentStatus.Approved);
            TotalResult result = DocumentTotalCalculator.Calculate(request);
            CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("uk-UA");
            string ukrainian = DocumentReport.TextOf(request, result);
            CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("en-US");
            Assert.Equal(ukrainian, DocumentReport.TextOf(request, result));
            Assert.Contains("Разом: 420.00\n", ukrainian);
            Assert.Equal(DocumentStatus.Approved, request.Status);
        }
        finally
        {
            CultureInfo.CurrentCulture = previous;
        }
    }

    [Fact]
    public void PrinterWritesExactTextToInjectedWriter()
    {
        using StringWriter output = new(CultureInfo.InvariantCulture);
        DocumentReportPrinter printer = new(output);
        printer.Print("Перевірка\n");
        Assert.Equal("Перевірка\n", output.ToString());
    }

    [Fact]
    public void PrinterRejectsNullWriterAndBlankText()
    {
        Assert.Throws<ArgumentNullException>(() => new DocumentReportPrinter(null!));
        DocumentReportPrinter printer = new(TextWriter.Null);
        Assert.Throws<ArgumentException>(() => printer.Print(" "));
    }

    [Fact]
    public void ReportRejectsInvalidResult()
    {
        TotalRequest request = TestData.Request();
        Assert.Throws<ArgumentNullException>(() => DocumentReport.TextOf(request, null!));
        Assert.Throws<ArgumentOutOfRangeException>(() => DocumentReport.TextOf(request, new TotalResult(-1m, DocumentStatus.New)));
        Assert.Throws<ArgumentOutOfRangeException>(() => DocumentReport.TextOf(request, new TotalResult(1m, (DocumentStatus)99)));
    }
}
