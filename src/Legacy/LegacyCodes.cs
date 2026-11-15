namespace WarehouseStock.Legacy;

public enum ClientKind { Regular, Vip, Staff }
public enum DocState { Other, New, Paid }

public static class LegacyCodes
{
    public static ClientKind Client(string? raw) => raw switch
    {
        "vip" => ClientKind.Vip,
        "staff" => ClientKind.Staff,
        _ => ClientKind.Regular,
    };

    public static DocState State(string? raw) => raw switch
    {
        "new" => DocState.New,
        "paid" => DocState.Paid,
        _ => DocState.Other,
    };
}
