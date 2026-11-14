using System.Globalization;
using System.Text;
using WarehouseStock.Core.Documents;

namespace WarehouseStock.Core.Reporting;

/// <summary>Побудова тексту без друку й зміни документа.</summary>
public static class DocumentReport
{
    /// <summary>Створює звіт з незмінним числовим форматом і символом нового рядка LF.</summary>
    /// <param name="request">Джерело відомостей про переміщення.</param>
    /// <param name="result">Перевірений результат розрахунку.</param>
    /// <returns>Повний текст звіту.</returns>
    public static string TextOf(TotalRequest request, TotalResult result)
    {
        DocumentGuards.EnsureRequestValid(request);
        DocumentGuards.EnsureResultValid(result);
        StringBuilder text = new();
        text.Append("Документ № ")
            .Append(request.DocumentId.ToString(CultureInfo.InvariantCulture)).Append('\n');
        text.Append("Одержувач: ").Append(request.Recipient.Name).Append('\n');
        text.Append("Пошта: ").Append(request.Recipient.Email).Append('\n');
        foreach (TransferLine line in request.Lines)
        {
            text.Append(line.Sku).Append(" x ")
                .Append(line.Quantity.ToString(CultureInfo.InvariantCulture)).Append('\n');
        }

        text.Append("Разом: ")
            .Append(result.Total.ToString("F2", CultureInfo.InvariantCulture)).Append('\n');
        text.Append("Стан: ").Append(result.NewStatus).Append('\n');
        return text.ToString();
    }
}
