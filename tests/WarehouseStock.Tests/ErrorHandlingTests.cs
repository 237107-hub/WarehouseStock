using System.Globalization;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using WarehouseStock.App.Infrastructure;
using WarehouseStock.App.Input;
using WarehouseStock.Core.Abstractions;
using WarehouseStock.Core.Domain;
using WarehouseStock.Core.Errors;
using WarehouseStock.Core.Notifications;
using WarehouseStock.Core.Pricing;
using WarehouseStock.Core.Services;
using WarehouseStock.Core.Storage;
using TransferLine = WarehouseStock.Core.Domain.TransferLine;

namespace WarehouseStock.Tests;

public sealed class ErrorHandlingTests
{
    [Theory]
    [InlineData("A", "дві", "10")]
    [InlineData("A", "0", "10")]
    [InlineData("A", "-1", "10")]
    [InlineData("A", "9999999999999999999999", "10")]
    [InlineData("", "1", "10")]
    [InlineData(null, "1", "10")]
    [InlineData("A", "1", "-1")]
    [InlineData("A", "1", "10,50")]
    [InlineData("A", "1", "1e2")]
    [InlineData("A", "1", null)]
    public void ParserWithInvalidInputReturnsFailureWithoutThrowing(string? sku, string? quantity, string? price)
    {
        Result<TransferLine> result = TransferLineParser.Parse(sku, quantity, price);
        Assert.False(result.IsSuccess);
        Assert.NotEmpty(result.Error);
    }

    [Fact]
    public void ParserUsesInvariantPointAndAllowsZeroPrice()
    {
        CultureInfo previous = CultureInfo.CurrentCulture;
        try
        {
            CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("uk-UA");
            Result<TransferLine> first = TransferLineParser.Parse(" A ", "2", "10.50");
            Assert.True(first.IsSuccess);
            Assert.Equal(21m, first.Value.Amount);
            Assert.Equal("A", first.Value.Sku);
            Assert.True(TransferLineParser.Parse("A", "1", "0").IsSuccess);
        }
        finally
        {
            CultureInfo.CurrentCulture = previous;
        }
    }

    [Fact]
    public void InputRepeatsAfterBadQuantityAndStopsAtEof()
    {
        using StringReader input = new("A\nдві\n10\nA\n2\n10.50\n");
        using StringWriter output = new(CultureInfo.InvariantCulture);
        Result<TransferLine> parsed = LineInput.ReadValidLine(input, output);
        Assert.True(parsed.IsSuccess);
        Assert.Equal(2, parsed.Value.Quantity);
        Assert.Contains("Помилка вводу: Кількість — ціле число більше 0", output.ToString());
        using StringReader empty = new(string.Empty);
        Assert.False(LineInput.ReadValidLine(empty, output).IsSuccess);
    }

    [Fact]
    public void FailedAndDefaultResultsProtectValueAccess()
    {
        Result<int> failed = ResultFactory.Fail<int>("Помилка");
        Assert.Throws<InvalidOperationException>(() => failed.Value);
        Result<int> empty = default;
        Assert.False(empty.IsSuccess);
        Assert.NotEmpty(empty.Error);
        Assert.Throws<InvalidOperationException>(() => empty.Value);
        Assert.Equal(7, ResultFactory.Ok(7).Value);
        Assert.Throws<ArgumentNullException>(() => ResultFactory.Ok<string>(null!));
        Assert.Throws<ArgumentException>(() => ResultFactory.Fail<int>(" "));
    }

    [Fact]
    public void DomainExceptionPreservesRuleContextAndInnerCause()
    {
        IOException cause = new("Диск недоступний.");
        DomainRuleException error = new("transfer.storage", "Переміщення 7 не прочитано.", cause);
        Assert.Equal("transfer.storage", error.Rule);
        Assert.Contains("7", error.Message);
        Assert.Same(cause, error.InnerException);
    }

    [Fact]
    public void MissingTransferIsDomainRuleWhileRepositorySearchRemainsOptional()
    {
        InMemoryTransferRepository repository = new();
        TransferService service = Service(repository);
        Assert.Null(repository.GetById(42));
        DomainRuleException error = Assert.Throws<DomainRuleException>(() => service.Load(42));
        Assert.Equal("transfer.exists", error.Rule);
        Assert.Contains("42", error.Message);
    }

    [Fact]
    public void ApprovalWithInsufficientLimitDoesNotChangeState()
    {
        InMemoryTransferRepository repository = new();
        TransferService service = Service(repository);
        Transfer transfer = Document();
        repository.Add(transfer);
        DomainRuleException error = Assert.Throws<DomainRuleException>(() => service.Approve(1, 100m));
        Assert.Equal("transfer.amount", error.Rule);
        Assert.Contains("100", error.Message);
        Assert.Contains("180", error.Message);
        Assert.Equal(TransferStatus.New, transfer.Status);
    }

    [Fact]
    public void ServiceRejectsClosedEditingAndInvalidCancellationAsNamedRules()
    {
        InMemoryTransferRepository repository = new();
        TransferService service = Service(repository);
        Transfer transfer = Document();
        repository.Add(transfer);
        service.Approve(1, 180m);
        Assert.Equal("transfer.editable", Assert.Throws<DomainRuleException>(() => service.AddLine(1, new TransferLine("B", 1, 1m))).Rule);
        transfer.Dispatch();
        Assert.Equal("transfer.cancellable", Assert.Throws<DomainRuleException>(() => service.Cancel(1, "Причина")).Rule);
    }

    [Fact]
    public void LoggingCapturesFourLevelsFieldsExceptionsAndDocumentScope()
    {
        TestLogger logger = new();
        DemoTransferRepository repository = new(new InMemoryTransferRepository());
        TransferService service = Service(repository, logger);
        service.Place(Document());
        repository.FailNextRead();
        Assert.Throws<IOException>(() => service.Approve(1, 200m));
        service.Approve(1, 200m);
        Assert.All(new[] { LogLevel.Debug, LogLevel.Information, LogLevel.Warning, LogLevel.Error },
            level => Assert.Contains(logger.Entries, entry => entry.Level == level));
        CapturedLog error = Assert.Single(logger.Entries, entry => entry.Level == LogLevel.Error);
        Assert.IsType<IOException>(error.Exception);
        Assert.Equal(1, (int)error.Fields["TransferId"]!);
        Assert.Contains("Transfer:1", error.Scopes);
        Assert.Contains(logger.Entries, entry => entry.Fields.TryGetValue("Extra", out object? extra) && Equals(extra, 20m));
        Assert.Equal(0, logger.ActiveScopes);
    }

    [Fact]
    public void IoRethrowKeepsOriginalInstanceAndStackFrame()
    {
        IOException original = new("Тестове сховище недоступне.");
        TransferService service = Service(new FaultRepository(original));
        IOException actual = Assert.Throws<IOException>(() => service.Load(1));
        Assert.Same(original, actual);
        Assert.Contains(nameof(FaultRepository.GetById), actual.StackTrace);
    }

    [Fact]
    public void ProgrammerDefectsAreNotSilenced()
    {
        Assert.Throws<ArgumentNullException>(() => new TransferService(null!, new RegularPricingPolicy(),
            new TextWriterNotifier(TextWriter.Null), NullLogger<TransferService>.Instance));
        TransferService service = Service(new InMemoryTransferRepository());
        Assert.Throws<ArgumentNullException>(() => service.AddLine(1, null!));
        TransferService invalid = new(new InMemoryTransferRepository(), new NegativePolicy(),
            new TextWriterNotifier(TextWriter.Null), NullLogger<TransferService>.Instance);
        Assert.Throws<InvalidOperationException>(() => invalid.TotalOf(Document()));
    }

    [Fact]
    public void QuantityCounterSkipsInvalidAndOverflowingRowsWithoutExceptions()
    {
        string[] rows = { "1", "дві", "-1", "0", "2", "99999999999999999999" };
        Assert.Equal(2, QuantityCounter.CountValidQuantities(rows));
    }

    private static Transfer Document()
    {
        Transfer transfer = new(1, 10, 20, new DateOnly(2026, 11, 14));
        transfer.AddLine(new TransferLine("A", 2, 100m));
        return transfer;
    }

    private static TransferService Service(ITransferRepository repository, ILogger<TransferService>? logger = null) =>
        new(repository, new DiscountPricingPolicy(0.10m), new TextWriterNotifier(TextWriter.Null),
            logger ?? NullLogger<TransferService>.Instance);

    private sealed class NegativePolicy : IPricingPolicy
    {
        public decimal PriceOf(TransferLine line) => -1m;
    }

    private sealed class FaultRepository(IOException failure) : ITransferRepository
    {
        public void Add(Transfer transfer) => throw failure;

        [MethodImpl(MethodImplOptions.NoInlining)]
        public Transfer? GetById(int id) => throw failure;

        public IReadOnlyList<Transfer> GetAll() => throw failure;
    }
}
