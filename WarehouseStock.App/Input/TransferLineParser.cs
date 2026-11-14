using System.Globalization;
using WarehouseStock.Core.Domain;
using WarehouseStock.Core.Errors;

namespace WarehouseStock.App.Input;

/// <summary>Барикада між текстовим вводом і незмінною позицією ядра.</summary>
public static class TransferLineParser
{
    private const NumberStyles PriceStyles = NumberStyles.AllowLeadingSign
        | NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite;

    /// <summary>Перевіряє рядки без винятків для звичайних помилок користувача.</summary>
    /// <param name="sku">Артикул товару.</param>
    /// <param name="quantity">Ціла додатна кількість.</param>
    /// <param name="price">Невід’ємна ціна з десятковою крапкою.</param>
    /// <returns>Позиція або пояснення, яке можна показати й повторити ввід.</returns>
    public static Result<TransferLine> Parse(string? sku, string? quantity, string? price)
    {
        if (string.IsNullOrWhiteSpace(sku))
        {
            return ResultFactory.Fail<TransferLine>("Артикул порожній");
        }

        if (!int.TryParse(quantity, NumberStyles.Integer, CultureInfo.InvariantCulture, out int q) || q <= 0)
        {
            return ResultFactory.Fail<TransferLine>("Кількість — ціле число більше 0");
        }

        if (!decimal.TryParse(price, PriceStyles, CultureInfo.InvariantCulture, out decimal p) || p < 0m)
        {
            return ResultFactory.Fail<TransferLine>("Ціна — невід’ємне число з крапкою");
        }

        return ResultFactory.Ok(new TransferLine(sku.Trim(), q, p));
    }
}
