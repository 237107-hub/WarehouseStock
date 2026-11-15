using System;
using System.Collections.Generic;

namespace WarehouseStock.Legacy;

public class WarehouseClient
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Email { get; set; } = "";
    public ClientKind Kind { get; set; } = ClientKind.Regular;
    public int DoneCount { get; set; }
    public DateTime SinceUtc { get; set; }
}

public class TransferItem
{
    public string Code { get; set; } = "";
    public int Qty { get; set; }
    public decimal Price { get; set; }
    public string Group { get; set; } = "";
}

public class LegacyTransferProcessor
{
    private decimal _tmpSum;
    private decimal _tmpDiscount;
    private readonly List<string> _log = new();

    public string Handle(int docId, int clientId,
        string clientName, string? clientMail,
        ClientKind clientKind, int clientDone,
        List<TransferItem>? items, DocState state,
        string currency, DateTime createdAt, bool sendMail)
    {
        if (items != null)
        {
            if (items.Count > 0)
            {
                if (state == DocState.New || state == DocState.Paid)
                {
                    if (clientMail != null && clientMail.Contains("@"))
                    {
                        _log.Add("ok " + docId);
                    }
                    else
                    {
                        return "ERR: mail";
                    }
                }
                else
                {
                    return "ERR: state";
                }
            }
            else
            {
                return "ERR: empty";
            }
        }
        else
        {
            return "ERR: null";
        }

        _tmpSum = 0m;
        for (int i = 0; i < items.Count; i++)
        {
            _tmpSum += items[i].Qty * items[i].Price;
        }

        _tmpDiscount = 0m;
        if (clientKind == ClientKind.Vip)
        {
            _tmpDiscount = _tmpSum * PricingRules.VipRate;
            if (_tmpDiscount > PricingRules.MaximumDiscount) _tmpDiscount = PricingRules.MaximumDiscount;
        }
        else if (clientKind == ClientKind.Staff)
        {
            _tmpDiscount = _tmpSum * PricingRules.StaffRate;
            if (_tmpDiscount > PricingRules.MaximumDiscount) _tmpDiscount = PricingRules.MaximumDiscount;
        }
        else if (clientDone > PricingRules.LoyaltyThreshold)
        {
            _tmpDiscount = _tmpSum * PricingRules.LoyalRate;
            if (_tmpDiscount > PricingRules.MaximumDiscount) _tmpDiscount = PricingRules.MaximumDiscount;
        }

        decimal ship = 0m;
        if (_tmpSum - _tmpDiscount < PricingRules.FreeShippingThreshold) ship = PricingRules.DeliveryFee;
        decimal total = _tmpSum - _tmpDiscount + ship;

        string txt = "Документ #" + docId + "\n";
        txt += "Клієнт: " + clientName + "\n";
        for (int i = 0; i < items.Count; i++)
        {
            txt += items[i].Code + " x" + items[i].Qty + " = " +
                (items[i].Qty * items[i].Price).ToString("0.00") + " " + currency + "\n";
        }

        txt += "Знижка: " + _tmpDiscount.ToString("0.00") + " " + currency + "\n";
        txt += "Доставка: " + ship.ToString("0.00") + " " + currency + "\n";
        txt += "Разом: " + total.ToString("0.00") + " " + currency + "\n";
        if (sendMail)
        {
            _log.Add("mail -> " + clientMail);
        }

        return txt;
    }

    public decimal Preview(ClientKind clientKind, int clientDone, List<TransferItem> items)
    {
        decimal s = 0m;
        foreach (var it in items)
        {
            s += it.Qty * it.Price;
        }

        decimal d = 0m;
        if (clientKind == ClientKind.Vip)
        {
            d = s * PricingRules.VipRate;
            if (d > PricingRules.MaximumDiscount) d = PricingRules.MaximumDiscount;
        }
        else if (clientKind == ClientKind.Staff)
        {
            d = s * PricingRules.StaffRate;
            if (d > PricingRules.MaximumDiscount) d = PricingRules.MaximumDiscount;
        }
        else if (clientDone > PricingRules.LoyaltyThreshold)
        {
            d = s * PricingRules.LoyalRate;
            if (d > PricingRules.MaximumDiscount) d = PricingRules.MaximumDiscount;
        }

        decimal sh = 0m;
        if (s - d < PricingRules.FreeShippingThreshold) sh = PricingRules.DeliveryFee;
        return s - d + sh;
    }

    public string DescribeClient(WarehouseClient c)
    {
        string s = c.Name.Trim().ToUpper();
        if (c.Kind == ClientKind.Vip) s += " [VIP]";
        if (c.DoneCount > PricingRules.LoyaltyThreshold) s += " [ЛОЯЛЬНИЙ]";
        s += " <" + c.Email.ToLower() + ">";
        int years = DateTime.Now.Year - c.SinceUtc.Year;
        s += " стаж " + years;
        return s;
    }

    public string DumpLog()
    {
        string r = "";
        foreach (var l in _log) r += l + "\n";
        return r;
    }
}
