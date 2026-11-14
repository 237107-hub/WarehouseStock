using System.Globalization;
using System.Text;
using WarehouseStock.App.Input;
using WarehouseStock.Core.Domain;
using WarehouseStock.Core.Errors;
using WarehouseStock.Core.Notifications;
using WarehouseStock.Core.Pricing;
using WarehouseStock.Core.Services;
using WarehouseStock.Core.Storage;

Console.OutputEncoding = Encoding.UTF8;
TransferService service = new(
    new InMemoryTransferRepository(),
    new DiscountPricingPolicy(0.10m),
    new TextWriterNotifier(Console.Out));
Transfer transfer = new(1, 10, 20, new DateOnly(2026, 11, 14));
service.Place(transfer);
using StringReader demonstration = new("SKU-1\nдві\n100\nSKU-1\n2\n100\n");
TextReader input = args.Contains("--interactive", StringComparer.Ordinal) ? Console.In : demonstration;
Result<TransferLine> result = LineInput.ReadValidLine(input, Console.Out);
if (!result.IsSuccess)
{
    Console.WriteLine(result.Error);
    return 1;
}

service.AddLine(transfer.Id, result.Value);
service.Approve(transfer.Id, 200m);
Console.WriteLine("Сума: " + service.TotalOf(transfer).ToString("F2", CultureInfo.InvariantCulture));
return 0;
