using System.Globalization;
using System.Text;
using WarehouseStock.Legacy;

Console.OutputEncoding = Encoding.UTF8;
CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
var processor = new LegacyTransferProcessor();
var request = new TransferRequest
{
    DocumentId = 1001,
    Client = new WarehouseClient { Name = "Іван", Email = "a@b", Kind = ClientKind.Vip },
    Items = new() { new() { Code = "A-1", Qty = 2, Price = 150m },
        new() { Code = "B-2", Qty = 1, Price = 400m } },
    State = DocState.New,
    Currency = "UAH",
    SendMail = true,
};
Console.WriteLine("ЛР6 / WarehouseStock / варіант 6");
Console.Write(processor.Handle(request));
Console.WriteLine("Журнал (імітація надсилання, без реального листа):");
Console.Write(processor.DumpLog());
Console.WriteLine("Помилка порожнього документа:");
Console.WriteLine(processor.Handle(request with { Items = new() }));
