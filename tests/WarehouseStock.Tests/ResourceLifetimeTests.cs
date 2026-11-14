using Microsoft.Extensions.Logging.Abstractions;
using WarehouseStock.App.Infrastructure;
using WarehouseStock.Core.Abstractions;
using WarehouseStock.Core.Domain;
using WarehouseStock.Core.Notifications;
using WarehouseStock.Core.Services;
using WarehouseStock.Core.Storage;
using TransferLine = WarehouseStock.Core.Domain.TransferLine;

namespace WarehouseStock.Tests;

public sealed class ResourceLifetimeTests
{
    [Fact]
    public void AuditFileClosesEvenWhenOperationThrowsAndDisposeIsIdempotent()
    {
        using TestDirectory folder = new();
        string path = Path.Combine(folder.Path, "audit.log");
        FileAuditLog audit = new(path);
        Action failingOperation = () =>
        {
            using (audit)
            {
                audit.Write("approve", 1);
                throw new IOException("Збій після запису.");
            }
        };
        Assert.Throws<IOException>(failingOperation);
        audit.Dispose();
        Assert.Throws<ObjectDisposedException>(() => audit.Write("again", 1));
        using FileStream exclusive = File.Open(path, FileMode.Open, FileAccess.ReadWrite, FileShare.None);
        Assert.True(exclusive.Length > 0);
    }

    [Fact]
    public void ExportStreamClosesWhenPricingThrowsAfterOpeningFile()
    {
        using TestDirectory folder = new();
        string path = Path.Combine(folder.Path, "report.txt");
        TransferService service = new(new InMemoryTransferRepository(), new ThrowingPolicy(),
            new TextWriterNotifier(TextWriter.Null), NullLogger<TransferService>.Instance);
        Transfer transfer = new(1, 10, 20, new DateOnly(2026, 11, 14));
        transfer.AddLine(new TransferLine("A", 1, 1m));
        Assert.Throws<ArithmeticException>(() => TransferReportExporter.ExportReport(path, transfer, service));
        using FileStream exclusive = File.Open(path, FileMode.Open, FileAccess.ReadWrite, FileShare.None);
        Assert.True(exclusive.Length > 0);
    }

    [Fact]
    public void AuditRejectsMultilineFieldsAndAppendsInsteadOfOverwriting()
    {
        using TestDirectory folder = new();
        string path = Path.Combine(folder.Path, "audit.log");
        using (FileAuditLog first = new(path))
        {
            Assert.Throws<ArgumentException>(() => first.Write("bad\nrecord", 1));
            first.Write("first", 1);
        }

        using (FileAuditLog second = new(path))
        {
            second.Write("second", 1);
        }

        Assert.Equal(2, File.ReadAllLines(path).Length);
    }

    private sealed class ThrowingPolicy : IPricingPolicy
    {
        public decimal PriceOf(TransferLine line) => throw new ArithmeticException("Тестовий збій ціни.");
    }

    private sealed class TestDirectory : IDisposable
    {
        public TestDirectory()
        {
            Path = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "WarehouseStockLab05-" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(Path);
        }

        public string Path { get; }

        public void Dispose()
        {
            foreach (string file in Directory.EnumerateFiles(Path))
            {
                File.Delete(file);
            }

            Directory.Delete(Path);
            GC.SuppressFinalize(this);
        }
    }
}
