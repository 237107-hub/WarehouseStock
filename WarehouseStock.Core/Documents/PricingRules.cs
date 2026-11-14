namespace WarehouseStock.Core.Documents;

/// <summary>Іменовані правила вартості переміщення.</summary>
public static class PricingRules
{
    /// <summary>Мінімальна гуртова кількість.</summary>
    public const int BulkQuantity = 10;

    /// <summary>Ставка гуртової знижки.</summary>
    public const decimal BulkRate = 0.05m;

    /// <summary>Знижка постійному одержувачу.</summary>
    public const decimal RegularClientRate = 0.07m;

    /// <summary>Строга нижня межа великого документа.</summary>
    public const decimal LargeOrderFrom = 5000m;

    /// <summary>Ставка великого документа.</summary>
    public const decimal LargeOrderRate = 0.10m;

    /// <summary>Строга нижня межа середнього документа.</summary>
    public const decimal MediumOrderFrom = 2000m;

    /// <summary>Ставка середнього документа.</summary>
    public const decimal MediumOrderRate = 0.05m;

    /// <summary>Код відсоткової знижки.</summary>
    public const string SaleCoupon = "SALE10";

    /// <summary>Ставка знижки SALE10.</summary>
    public const decimal CouponSaleRate = 0.10m;

    /// <summary>Код фіксованої знижки.</summary>
    public const string FixedCoupon = "MINUS200";

    /// <summary>Строга нижня межа фіксованої знижки.</summary>
    public const decimal CouponFixedFrom = 1000m;

    /// <summary>Розмір фіксованої знижки.</summary>
    public const decimal CouponFixed = 200m;

    /// <summary>Недільна знижка.</summary>
    public const decimal WeekendRate = 0.02m;

    /// <summary>Межа безоплатної доставки.</summary>
    public const decimal FreeDeliveryFrom = 1000m;
}
