namespace WarehouseStock.Legacy;

public static class DiscountPolicy
{
    public static decimal Calculate(decimal subtotal, ClientKind clientKind, int clientDone)
    {
        decimal discount = 0m;
        if (clientKind == ClientKind.Vip)
        {
            discount = subtotal * PricingRules.VipRate;
            if (discount > PricingRules.MaximumDiscount)
            {
                discount = PricingRules.MaximumDiscount;
            }
        }
        else if (clientKind == ClientKind.Staff)
        {
            discount = subtotal * PricingRules.StaffRate;
            if (discount > PricingRules.MaximumDiscount)
            {
                discount = PricingRules.MaximumDiscount;
            }
        }
        else if (clientDone > PricingRules.LoyaltyThreshold)
        {
            discount = subtotal * PricingRules.LoyalRate;
            if (discount > PricingRules.MaximumDiscount)
            {
                discount = PricingRules.MaximumDiscount;
            }
        }

        return discount;
    }

}
