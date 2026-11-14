using System.Text;
using WarehouseStock.Core.Legacy;

Console.OutputEncoding = Encoding.UTF8;
var z = new peremishchennia("A-1001", "Центральний склад");
z.Add("SKU-1", 3, 250m);
z.Add("SKU-2", 12, 90m);
Console.WriteLine(z.ProcessData(false));
Console.WriteLine(z.ProcessData(true));
Console.WriteLine(z.Ck());
Console.WriteLine(z.Chg(1));
Console.Write(z.Rep());
