using WarehouseStock.Core.Documents;
using WarehouseStock.Tests.Reference;

namespace WarehouseStock.Tests;

public sealed class DifferentialTests
{
    [Fact]
    public void TwoHundredValidDocumentsMatchPinnedLegacyReference()
    {
        Random random = new(47106);
        string[] coupons = { string.Empty, "SALE10", "MINUS200", "UNKNOWN" };
        for (int i = 0; i < 200; i++)
        {
            List<TransferLine> lines = new()
            {
                new TransferLine { Sku = "A1", Quantity = random.Next(1, 21), UnitPrice = random.Next(0, 50001) / 100m },
                new TransferLine { Sku = "B2", Quantity = random.Next(1, 21), UnitPrice = random.Next(0, 50001) / 100m },
            };
            TotalRequest request = TestData.Request(
                coupon: coupons[i % coupons.Length], status: (DocumentStatus)(i % 4), date: new DateOnly(2026, 3, 8 + (i % 3))) with
            {
                Lines = lines,
                Recipient = new RecipientInfo("Склад", "w@example.com", i % 2 == 0),
            };
            decimal expected = LegacyReference.Process(
                request.DocumentId, request.Recipient.Name, request.Recipient.Email, request.Recipient.IsRegular, lines,
                request.CreatedAt, request.CouponCode!, request.Status.ToString(), false, request.DeliveryPrice, out string status);
            TotalResult actual = DocumentTotalCalculator.Calculate(request);
            Assert.Equal(expected, actual.Total);
            Assert.Equal(status, actual.NewStatus.ToString());
        }
    }
}
