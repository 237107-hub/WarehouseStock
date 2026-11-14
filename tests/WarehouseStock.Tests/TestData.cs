using WarehouseStock.Core.Documents;

namespace WarehouseStock.Tests;

internal static class TestData
{
    public static TotalRequest Request(
        decimal amount = 400m,
        string? coupon = null,
        DocumentStatus status = DocumentStatus.New,
        DateOnly? date = null) => new()
        {
            DocumentId = 2,
            Recipient = new RecipientInfo("Західний склад", "west@example.com", false),
            Lines = new[] { new TransferLine { Sku = "A1", Quantity = 1, UnitPrice = amount } },
            CreatedAt = date ?? new DateOnly(2026, 3, 10),
            CouponCode = coupon,
            Status = status,
            DeliveryPrice = 60m,
        };
}
