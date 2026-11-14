using System.Globalization;
using WarehouseStock.Core.Domain;

Transfer transfer = new()
{
    Id = 1,
    SourceWarehouseId = 10,
    DestinationWarehouseId = 20,
    CreatedAt = DateTimeOffset.Now,
};

transfer.AddLine(new TransferLine
{
    Sku = "SKU-001",
    Quantity = 2,
    UnitPrice = 149.50m,
});

transfer.AddLine(new TransferLine
{
    Sku = "SKU-002",
    Quantity = 1,
    UnitPrice = 89.90m,
});

string total = transfer.Total().ToString("F2", CultureInfo.InvariantCulture);
Console.WriteLine($"Transfer #{transfer.Id}");
Console.WriteLine($"Route: {transfer.SourceWarehouseId} -> {transfer.DestinationWarehouseId}");
Console.WriteLine($"Status: {transfer.Status}");
Console.WriteLine($"Total: {total}");
