using System.Globalization;

namespace WarehouseStock.Tests.Legacy;

// Порушення SOLID збережені навмисно. Код знаходиться поза Core.
public sealed class TransferManager
{
    private readonly FileTransferStore _store = new();
    private readonly SmtpMailer _mailer = new();

    public decimal Place(LegacyTransfer transfer, string recipientType)
    {
        decimal total = 0m;
        foreach (LegacyLine line in transfer.Lines)
        {
            total += line.UnitPrice * line.Quantity;
        }

        if (recipientType == "regular")
        {
            total *= 0.95m;
        }
        else if (recipientType == "vip")
        {
            total *= 0.90m;
        }
        else if (recipientType == "staff")
        {
            total *= 0.70m;
        }

        if (total > 1000m)
        {
            total -= 50m;
        }

        transfer.Total = total;
        transfer.Status = 1;
        _store.SaveToFile(transfer, "transfers.json");
        _mailer.Send(transfer.Email, "Transfer " + transfer.Id.ToString(CultureInfo.InvariantCulture),
            total.ToString(CultureInfo.InvariantCulture));
        return total;
    }

    public static string BuildCsvReport(List<LegacyTransfer> transfers)
    {
        string text = "id;total;status" + Environment.NewLine;
        foreach (LegacyTransfer transfer in transfers)
        {
            text += transfer.Id.ToString(CultureInfo.InvariantCulture) + ";"
                + transfer.Total.ToString(CultureInfo.InvariantCulture) + ";"
                + transfer.Status.ToString(CultureInfo.InvariantCulture);
        }

        File.WriteAllText("report.csv", text);
        Console.WriteLine("Звіт збережено");
        return text;
    }

    public void Archive(LegacyTransfer transfer)
    {
        LegacyTransfer archived = new ArchivedLegacyTransfer(transfer.Id);
        foreach (LegacyLine line in transfer.Lines)
        {
            archived.AddLine(line);
        }

        _store.SaveToFile(archived, "archive.json");
    }
}
