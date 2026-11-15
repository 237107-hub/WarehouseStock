using WarehouseStock.Core.Abstractions;
using WarehouseStock.Core.Domain;

namespace WarehouseStock.Tests.Lab07;

internal sealed class StubPricingPolicy : IPricingPolicy
{
    private readonly decimal _price;

    public StubPricingPolicy(decimal price) => _price = price;

    public decimal PriceOf(TransferLine line) => _price;
}
