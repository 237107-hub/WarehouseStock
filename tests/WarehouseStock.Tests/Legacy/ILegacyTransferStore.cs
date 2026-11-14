namespace WarehouseStock.Tests.Legacy;

public interface ILegacyTransferStore
{
    void Add(LegacyTransfer transfer);

    LegacyTransfer GetById(int id);

    void SaveToFile(LegacyTransfer transfer, string path);

    void SendEmail(string recipientAddress, string body);

    string ExportCsv();

    void Backup(string folder);

    void PrintInvoice(LegacyTransfer transfer);
}
