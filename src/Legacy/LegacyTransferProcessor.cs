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
        string? error = Validate(items, state, clientMail);
        if (error != null) return error;
        _log.Add("ok " + docId);

        _tmpSum = Subtotal(items!);
        _tmpDiscount = DiscountOf(_tmpSum, clientKind, clientDone);
        decimal ship = ShippingOf(_tmpSum - _tmpDiscount);
        decimal total = _tmpSum - _tmpDiscount + ship;

        string txt = "Документ #" + docId + "\n";
        txt += "Клієнт: " + clientName + "\n";
        for (int i = 0; i < items!.Count; i++)
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

    private static string? Validate(List<TransferItem>? items, DocState state, string? mail)
    {
        if (items == null) return "ERR: null";
        if (items.Count == 0) return "ERR: empty";
        if (state != DocState.New && state != DocState.Paid) return "ERR: state";
        if (mail == null || !mail.Contains('@')) return "ERR: mail";
        return null;
    }

    private static decimal Subtotal(List<TransferItem> items)
    {
        decimal sum = 0m;
        foreach (var item in items) sum += item.Qty * item.Price;
        return sum;
    }

    private static decimal DiscountOf(decimal subtotal, ClientKind clientKind, int clientDone)
    {
        decimal discount = 0m;
        if (clientKind == ClientKind.Vip)
        {
            discount = subtotal * PricingRules.VipRate;
            if (discount > PricingRules.MaximumDiscount) discount = PricingRules.MaximumDiscount;
        }
        else if (clientKind == ClientKind.Staff)
        {
            discount = subtotal * PricingRules.StaffRate;
            if (discount > PricingRules.MaximumDiscount) discount = PricingRules.MaximumDiscount;
        }
        else if (clientDone > PricingRules.LoyaltyThreshold)
        {
            discount = subtotal * PricingRules.LoyalRate;
            if (discount > PricingRules.MaximumDiscount) discount = PricingRules.MaximumDiscount;
        }

        return discount;
    }

    private static decimal ShippingOf(decimal payable)
    {
        decimal ship = 0m;
        if (payable < PricingRules.FreeShippingThreshold) ship = PricingRules.DeliveryFee;
        return ship;
    }

    public decimal Preview(ClientKind clientKind, int clientDone, List<TransferItem> items)
    {
        decimal sum = Subtotal(items);
        decimal discount = DiscountOf(sum, clientKind, clientDone);
        return sum - discount + ShippingOf(sum - discount);
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
