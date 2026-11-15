using WarehouseStock.Legacy;

namespace WarehouseStock.Legacy.Tests;

internal static class TestApi
{
    internal static string Handle(LegacyTransferProcessor processor, List<TransferItem>? items,
        string state = "new", string? mail = "a@b", string kind = "vip", int done = 0,
        bool send = false) => processor.Handle(new TransferRequest
        {
            DocumentId = 1001,
            Client = new WarehouseClient { Id = 7, Name = "Іван", Email = mail!,
                Kind = LegacyCodes.Client(kind), DoneCount = done },
            Items = items, State = LegacyCodes.State(state), Currency = "UAH", SendMail = send,
        });

    internal static decimal Preview(LegacyTransferProcessor processor, string kind, int done,
        List<TransferItem> items) => processor.Preview(LegacyCodes.Client(kind), done, items);

    internal static string Describe(LegacyTransferProcessor processor, string name,
        string kind, int done, string email) => processor.DescribeClient(new WarehouseClient
        { Name = name, Kind = LegacyCodes.Client(kind), DoneCount = done, Email = email, SinceUtc = new DateTime(2020, 1, 1) });
}
