using WarehouseStock.Core.Abstractions;
using WarehouseStock.Core.Domain;

namespace WarehouseStock.Core.Pricing;

/// <summary>Окреме розширення політик для пріоритетного одержувача.</summary>
public sealed class VipPricingPolicy : IPricingPolicy
{
    private const decimal Rate = 0.10m;

    /// <inheritdoc/>
    public decimal PriceOf(TransferLine line)
    {
        ArgumentNullException.ThrowIfNull(line);
        return Math.Round(line.Amount * (1m - Rate), 2);
    }
}
