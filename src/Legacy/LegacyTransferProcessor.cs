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

    public string Describe()
    {
        string s = Name.Trim().ToUpper(System.Globalization.CultureInfo.CurrentCulture);
        if (Kind == ClientKind.Vip) s += " [VIP]";
        if (DoneCount > PricingRules.LoyaltyThreshold) s += " [ЛОЯЛЬНИЙ]";
        s += " <" + Email.ToLower(System.Globalization.CultureInfo.CurrentCulture) + ">";
        int years = DateTime.Now.Year - SinceUtc.Year;
        s += " стаж " + years;
        return s;
    }
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

    public string Handle(TransferRequest request)
    {
        string? error = Validate(request.Items, request.State, request.Client.Email);
        if (error != null) return error;
        _log.Add("ok " + request.DocumentId);

        _tmpSum = Subtotal(request.Items!);
        _tmpDiscount = DiscountPolicy.Calculate(_tmpSum, request.Client.Kind, request.Client.DoneCount);
        decimal ship = ShippingPolicy.Calculate(_tmpSum - _tmpDiscount);
        decimal total = _tmpSum - _tmpDiscount + ship;

        string txt = ReportBuilder.Build(request.DocumentId, request.Client.Name, request.Items!, request.Currency, _tmpDiscount, ship, total);
        if (request.SendMail)
        {
            _log.Add("mail -> " + request.Client.Email);
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

    public decimal Preview(ClientKind clientKind, int clientDone, List<TransferItem> items)
    {
        decimal sum = Subtotal(items);
        decimal discount = DiscountPolicy.Calculate(sum, clientKind, clientDone);
        return sum - discount + ShippingPolicy.Calculate(sum - discount);
    }

    public string DumpLog()
    {
        string r = "";
        foreach (var l in _log) r += l + "\n";
        return r;
    }
}
