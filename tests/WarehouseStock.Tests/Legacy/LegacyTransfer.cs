namespace WarehouseStock.Tests.Legacy;

public class LegacyTransfer
{
    public int Id { get; set; }

    public int Status { get; set; }

    public decimal Total { get; set; }

    public string Email { get; set; } = string.Empty;

    public List<LegacyLine> Lines { get; set; } = new();

    public virtual void AddLine(LegacyLine line) => Lines.Add(line);
}
