namespace WarehouseStock.Legacy;

public static class PricingRules
{
    public const decimal VipRate = 0.15m;
    public const decimal StaffRate = 0.30m;
    public const decimal LoyalRate = 0.05m;
    public const decimal MaximumDiscount = 500m;
    public const decimal FreeShippingThreshold = 1000m;
    public const decimal DeliveryFee = 60m;
    public const int LoyaltyThreshold = 10;
}
