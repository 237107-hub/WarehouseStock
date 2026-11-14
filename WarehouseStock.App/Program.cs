using System.Text;
using WarehouseStock.Core.Documents;
using WarehouseStock.Core.Reporting;

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
string text = DocumentReport.TextOf(request, result);
Console.OutputEncoding = Encoding.UTF8;
DocumentReportPrinter printer = new(Console.Out);
printer.Print(text);
