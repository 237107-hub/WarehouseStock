using WarehouseStock.Core.Abstractions;
using WarehouseStock.Core.Domain;

namespace WarehouseStock.Core.Pricing;

/// <summary>Змінювана через конструктор ставка знижки до 50%.</summary>
public sealed class DiscountPricingPolicy : IPricingPolicy
{
    private readonly decimal _rate;

    /// <summary>Створює незмінну політику допустимої знижки.</summary>
    /// <param name="rate">Частка від 0 до 0.5 включно.</param>
    public DiscountPricingPolicy(decimal rate)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(rate);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(rate, 0.5m);
        _rate = rate;
    }

    /// <inheritdoc/>
    public decimal PriceOf(TransferLine line)
    {
        ArgumentNullException.ThrowIfNull(line);
        return Math.Round(line.Amount * (1m - _rate), 2);
    }
}
