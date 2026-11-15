namespace WarehouseStock.Core.Pricing;

/// <summary>Навчальна знижка на складське відвантаження для ЛР7.</summary>
public static class TransferDiscountPolicy
{
    /// <summary>Ставка для постійного одержувача.</summary>
    public const decimal RegularRate = 0.05m;

    /// <summary>Додаткова ставка для великого відвантаження.</summary>
    public const decimal BulkRate = 0.10m;

    /// <summary>Включна нижня межа великого відвантаження.</summary>
    public const decimal BulkFrom = 1000m;

    /// <summary>Максимальна знижка в гривнях.</summary>
    public const decimal MaximumDiscount = 150m;

    /// <summary>Обчислює суму знижки з округленням до копійок.</summary>
    /// <param name="amount">Невід'ємна вартість відвантаження.</param>
    /// <param name="isRegular">Ознака постійного одержувача.</param>
    /// <returns>Сума знижки.</returns>
    public static decimal DiscountFor(decimal amount, bool isRegular)
    {
        if (amount < 0m)
        {
            throw new ArgumentOutOfRangeException(nameof(amount), "Сума не може бути від’ємною.");
        }

        decimal rate = 0m;
        if (isRegular)
        {
            rate += RegularRate;
        }

        if (amount >= BulkFrom)
        {
            rate += BulkRate;
        }

        decimal discount = decimal.Round(amount * rate, 2);
        if (discount > MaximumDiscount)
        {
            discount = MaximumDiscount;
        }

        return discount;
    }
}
