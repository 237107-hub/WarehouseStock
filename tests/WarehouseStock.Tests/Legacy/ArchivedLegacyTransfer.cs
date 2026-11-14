namespace WarehouseStock.Tests.Legacy;

public sealed class ArchivedLegacyTransfer : LegacyTransfer
{
    public ArchivedLegacyTransfer(int id) => Id = id;

    public override void AddLine(LegacyLine line)
    {
        throw new NotSupportedException("Архів змінювати не можна.");
    }
}
