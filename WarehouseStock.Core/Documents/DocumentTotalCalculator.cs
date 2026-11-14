namespace WarehouseStock.Core.Documents;

/// <summary>Чисті підпрограми розрахунку переміщення.</summary>
public static class DocumentTotalCalculator
{
    /// <summary>Сума позицій з гуртовою знижкою.</summary>
    /// <param name="lines">Позиції переміщення.</param>
    /// <returns>Обчислене значення правила.</returns>
    public static decimal SubtotalOf(IReadOnlyList<TransferLine> lines)
    {
        DocumentGuards.EnsureLinesValid(lines);
        decimal subtotal = 0m;
        foreach (TransferLine line in lines)
        {
            decimal sum = line.Quantity * line.UnitPrice;
            if (line.Quantity >= PricingRules.BulkQuantity)
            {
                sum -= sum * PricingRules.BulkRate;
            }

            subtotal += sum;
        }

        return subtotal;
    }

    /// <summary>Знижка постійному одержувачу.</summary>
    /// <param name="amount">Сума до знижки.</param>
    /// <param name="isRegularClient">Ознака постійного одержувача.</param>
    /// <returns>Обчислене значення правила.</returns>
    public static decimal ClientDiscountOf(decimal amount, bool isRegularClient)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(amount);
        return isRegularClient ? amount * PricingRules.RegularClientRate : 0m;
    }

    /// <summary>Знижка за обсягом документа.</summary>
    /// <param name="amount">Сума до знижки.</param>
    /// <returns>Обчислене значення правила.</returns>
    public static decimal VolumeDiscountOf(decimal amount)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(amount);
        if (amount > PricingRules.LargeOrderFrom)
        {
            return amount * PricingRules.LargeOrderRate;
        }

        return amount > PricingRules.MediumOrderFrom ? amount * PricingRules.MediumOrderRate : 0m;
    }

    /// <summary>Знижка за відомим кодом купона.</summary>
    /// <param name="amount">Сума до знижки.</param>
    /// <param name="couponCode">Код купона або null для відсутнього купона.</param>
    /// <returns>Обчислене значення правила.</returns>
    public static decimal CouponDiscountOf(decimal amount, string? couponCode)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(amount);
        return couponCode switch
        {
            PricingRules.SaleCoupon => amount * PricingRules.CouponSaleRate,
            PricingRules.FixedCoupon when amount > PricingRules.CouponFixedFrom => PricingRules.CouponFixed,
            _ => 0m,
        };
    }

    /// <summary>Недільна знижка.</summary>
    /// <param name="amount">Сума до знижки.</param>
    /// <param name="createdAt">Явно передана дата документа.</param>
    /// <returns>Обчислене значення правила.</returns>
    public static decimal WeekendDiscountOf(decimal amount, DateOnly createdAt)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(amount);
        return createdAt.DayOfWeek == DayOfWeek.Sunday ? amount * PricingRules.WeekendRate : 0m;
    }

    /// <summary>Плата за доставку для невеликого документа.</summary>
    /// <param name="amount">Сума після знижок.</param>
    /// <param name="deliveryPrice">Невід’ємна плата за доставку.</param>
    /// <returns>Обчислене значення правила.</returns>
    public static decimal DeliveryFeeOf(decimal amount, decimal deliveryPrice)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(amount);
        ArgumentOutOfRangeException.ThrowIfNegative(deliveryPrice);
        return amount < PricingRules.FreeDeliveryFrom ? deliveryPrice : 0m;
    }

    /// <summary>Наступний стан без зміни вхідного документа.</summary>
    /// <param name="current">Допустимий поточний стан.</param>
    /// <param name="total">Невід’ємний підсумок.</param>
    /// <returns>Обчислене значення правила.</returns>
    public static DocumentStatus NextStatusOf(DocumentStatus current, decimal total)
    {
        DocumentGuards.EnsureStatusValid(current);
        ArgumentOutOfRangeException.ThrowIfNegative(total);
        return current switch
        {
            DocumentStatus.New when total > 0m => DocumentStatus.Approved,
            DocumentStatus.Approved => DocumentStatus.Dispatched,
            _ => current,
        };
    }
}
