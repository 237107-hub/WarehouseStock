using System.Globalization;
using WarehouseStock.Core.Documents;

TotalRequest request = new()
{
    DocumentId = 2,
    Recipient = new RecipientInfo("Західний склад", "west@example.com", false),
    Lines = new[] { new TransferLine { Sku = "A1", Quantity = 2, UnitPrice = 200m } },
    CreatedAt = new DateOnly(2026, 3, 10),
    CouponCode = "SALE10",
    Status = DocumentStatus.Approved,
    DeliveryPrice = 60m,
};
TotalResult result = DocumentTotalCalculator.Calculate(request);
Console.WriteLine(result.Total.ToString("F2", CultureInfo.InvariantCulture));
Console.WriteLine(result.NewStatus);
