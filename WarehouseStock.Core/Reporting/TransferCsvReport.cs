using System.Globalization;
using System.Text;

namespace WarehouseStock.Core.Reporting;

/// <summary>Чисте форматування CSV із перевірених рядків-знімків.</summary>
public static class TransferCsvReport
{
    /// <summary>Формує CSV із числовими полями та іменованим станом.</summary>
    /// <param name="rows">Колекція рядків без довільного тексту користувача.</param>
    /// <returns>CSV із роздільником ;, InvariantCulture та LF.</returns>
    public static string BuildCsv(IReadOnlyList<TransferReportRow> rows)
    {
        ArgumentNullException.ThrowIfNull(rows);
        StringBuilder text = new("id;total;status\n");
        foreach (TransferReportRow row in rows)
        {
            ArgumentNullException.ThrowIfNull(row);
            text.Append(row.Id.ToString(CultureInfo.InvariantCulture)).Append(';');
            text.Append(row.Total.ToString("F2", CultureInfo.InvariantCulture)).Append(';');
            text.Append(row.Status).Append('\n');
        }

        return text.ToString();
    }
}
