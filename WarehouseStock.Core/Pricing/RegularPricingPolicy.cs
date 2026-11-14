using WarehouseStock.Core.Abstractions;
using WarehouseStock.Core.Domain;

namespace WarehouseStock.Core.Pricing;

/// <summary>Облікова вартість без знижки.</summary>
public sealed class RegularPricingPolicy : IPricingPolicy
{
    /// <inheritdoc/>
    public decimal PriceOf(TransferLine line)
    {
        ArgumentNullException.ThrowIfNull(line);
        return Math.Round(line.Amount, 2);
    }
}
