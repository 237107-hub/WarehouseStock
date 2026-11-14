using System.Text;
using WarehouseStock.Core.Legacy;

Console.OutputEncoding = Encoding.UTF8;
StockTransfer transfer = new StockTransfer("A-1001", "Центральний склад");
transfer.AddLine("SKU-1", 3, 250m);
transfer.AddLine("SKU-2", 12, 90m);
Console.WriteLine(transfer.CalculateTotal(false));
Console.WriteLine(transfer.CalculateTotal(true));
Console.WriteLine(transfer.IsValid());
Console.WriteLine(transfer.TryChangeStatus(1));
Console.Write(transfer.BuildReport());
