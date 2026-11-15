namespace WarehouseStock.Legacy;

public sealed record TransferRequest
{
    public int DocumentId { get; init; }
    public WarehouseClient Client { get; init; } = new();
    public List<TransferItem>? Items { get; init; }
    public DocState State { get; init; }
    public string Currency { get; init; } = "";
    public bool SendMail { get; init; }
}
