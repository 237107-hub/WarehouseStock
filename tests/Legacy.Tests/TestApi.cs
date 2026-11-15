using WarehouseStock.Legacy;

namespace WarehouseStock.Legacy.Tests;

internal static class TestApi
{
    internal static string Handle(LegacyTransferProcessor processor, List<TransferItem>? items,
        string state = "new", string? mail = "a@b", string kind = "vip", int done = 0,
        bool send = false) => processor.Handle(1001, 7, "Іван", mail, kind, done,
            items, state, "UAH", new DateTime(2020, 1, 1), send);

    internal static decimal Preview(LegacyTransferProcessor processor, string kind, int done,
        List<TransferItem> items) => processor.Preview(kind, done, items);

    internal static string Describe(LegacyTransferProcessor processor, string name,
        string kind, int done, string email) => processor.DescribeClient(new WarehouseClient
        { Name = name, Kind = kind, DoneCount = done, Email = email, SinceUtc = new DateTime(2020, 1, 1) });
}
