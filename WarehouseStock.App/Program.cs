using System.Text;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using WarehouseStock.App;
using WarehouseStock.App.Diagnostics;
using WarehouseStock.App.Infrastructure;
using WarehouseStock.App.Input;
using WarehouseStock.Core.Errors;
using WarehouseStock.Core.Notifications;
using WarehouseStock.Core.Pricing;
using WarehouseStock.Core.Services;
using WarehouseStock.Core.Storage;

Console.OutputEncoding = Encoding.UTF8;
if (OperatingSystem.IsWindows())
{
    Console.Title = "WarehouseStock - ЛР5";
}

using ILoggerFactory factory = LoggerFactory.Create(builder =>
{
    builder.AddSimpleConsole(options =>
    {
        options.TimestampFormat = "HH:mm:ss ";
        options.IncludeScopes = true;
        options.ColorBehavior = LoggerColorBehavior.Disabled;
    });
    builder.SetMinimumLevel(LogLevel.Debug);
});
ILogger log = factory.CreateLogger("WarehouseStock.App");
int exitCode;
try
{
    exitCode = RunApplication(args, factory);
}
catch (DomainRuleException exception)
{
    ApplicationLog.Rule(log, exception.Rule, exception.Message);
    Console.WriteLine("Правило: " + exception.Message);
    exitCode = 1;
}
catch (IOException exception)
{
    ApplicationLog.IoFailure(log, exception);
    Console.WriteLine("Файл або сховище зайняті; закрийте файл і повторіть.");
    exitCode = 3;
}
catch (UnauthorizedAccessException exception)
{
    ApplicationLog.AccessDenied(log, exception);
    Console.WriteLine("Немає прав на запис у вибрану теку.");
    exitCode = 4;
}
catch (Exception exception)
{
    ApplicationLog.Unexpected(log, exception);
    Console.WriteLine("Внутрішня помилка; подробиці в журналі.");
    exitCode = 2;
}

if (!Console.IsInputRedirected && !args.Contains("--no-pause", StringComparer.Ordinal))
{
    Console.WriteLine("Натисніть Enter для завершення.");
    Console.ReadLine();
}

return exitCode;

static int RunApplication(string[] arguments, ILoggerFactory logFactory)
{
    DemoTransferRepository repository = new(new InMemoryTransferRepository());
    TransferService service = new(
        repository,
        new DiscountPricingPolicy(0.10m),
        new TextWriterNotifier(Console.Out),
        logFactory.CreateLogger<TransferService>());
    using DemonstrationInput demonstration = new(Console.Out);
    bool interactive = arguments.Contains("--interactive", StringComparer.Ordinal);
    TextReader input = interactive ? Console.In : demonstration;
    Action failure = interactive ? static () => { } : repository.FailNextRead;
    Console.WriteLine(interactive ? "Режим вводу." : "Демонстрація: помилка вводу та один штучний збій сховища.");
    TransferScenario scenario = new(service, Console.Out, failure);
    int result = scenario.Run(input);
    if (result == 0)
    {
        WriteReports(service);
    }

    return result;
}

static void WriteReports(TransferService service)
{
    string directory = Path.Combine(AppContext.BaseDirectory, "data");
    Directory.CreateDirectory(directory);
    using FileAuditLog audit = new(Path.Combine(directory, "audit.log"));
    audit.Write("approve", 1);
    TransferReportExporter.ExportReport(Path.Combine(directory, "transfer.txt"), service.Load(1), service);
    Console.WriteLine("Аудит і звіт збережено у " + directory);
}
