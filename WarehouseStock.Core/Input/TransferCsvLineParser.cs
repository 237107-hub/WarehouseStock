using WarehouseStock.Core.Domain;
using WarehouseStock.Core.Errors;

namespace WarehouseStock.Core.Input;

/// <summary>Розбирає зовнішній рядок sku;quantity;price через спільну барикаду.</summary>
public static class TransferCsvLineParser
{
    private const int MaximumRawLength = 256;

    /// <summary>Не створює сутність, доки всі три поля не перевірено.</summary>
    /// <param name="raw">Зовнішній рядок із трьома полями.</param>
    /// <param name="line">Перевірена позиція або null на невдачі.</param>
    /// <returns>Чи вдалося створити позицію.</returns>
    public static bool TryParse(string? raw, out TransferLine? line)
    {
        line = null;
        if (string.IsNullOrWhiteSpace(raw) || raw.Length > MaximumRawLength)
        {
            return false;
        }

        string[] fields = raw.Split(';');
        if (fields.Length != 3)
        {
            return false;
        }

        Result<TransferLine> result = TransferLineInput.ParseFields(fields[0], fields[1], fields[2]);
        if (!result.IsSuccess)
        {
            return false;
        }

        line = result.Value;
        return true;
    }
}
