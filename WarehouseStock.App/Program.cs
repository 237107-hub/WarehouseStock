using System.Globalization;
using System.Text;
using WarehouseStock.Core.Abstractions;
using WarehouseStock.Core.Domain;
using WarehouseStock.Core.Notifications;
using WarehouseStock.Core.Pricing;
using WarehouseStock.Core.Reporting;
using WarehouseStock.Core.Services;
using WarehouseStock.Core.Storage;

Console.OutputEncoding = Encoding.UTF8;
ITransferRepository repository = new InMemoryTransferRepository();
INotifier notifier = new TextWriterNotifier(Console.Out);
Dictionary<string, IPricingPolicy> policies = new()
{
    ["standard"] = new RegularPricingPolicy(),
    ["regular"] = new DiscountPricingPolicy(0.05m),
    ["vip"] = new VipPricingPolicy(),
    ["staff"] = new DiscountPricingPolicy(0.30m),
};
string recipientType = args.FirstOrDefault() ?? "regular";
IPricingPolicy pricing = policies.GetValueOrDefault(recipientType, policies["standard"]);
TransferService service = new(repository, pricing, notifier);
Warehouse source = new(10, "Центральний склад", "central@example.com");
Warehouse destination = new(20, "Західний склад", "west@example.com");
Product product = new("SKU-1", "Навчальний товар", 250m, 20);
Transfer transfer = new(1, source.Id, destination.Id, new DateOnly(2026, 11, 14));
transfer.AddLine(new TransferLine(product.Sku, 2, product.Price));
service.Place(transfer);
decimal total = service.TotalOf(transfer);
Console.WriteLine($"Маршрут: {source.Name} -> {destination.Name}");
Console.WriteLine("Сума: " + total.ToString("F2", CultureInfo.InvariantCulture));
transfer.Approve();
transfer.Dispatch();
transfer.Complete();
ArchivedTransfer archive = new(transfer, new DateOnly(2026, 11, 14));
Console.WriteLine($"Стан архіву: {archive.Status}");
TransferReportRow[] rows = { new(transfer.Id, total, transfer.Status) };
Console.Write(TransferCsvReport.BuildCsv(rows));
