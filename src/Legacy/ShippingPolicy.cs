namespace WarehouseStock.Legacy;

public static class ShippingPolicy
{
    public static decimal Calculate(decimal payable)
    {
        decimal ship = 0m;
        if (payable < PricingRules.FreeShippingThreshold) ship = PricingRules.DeliveryFee;
        return ship;
    }

}
