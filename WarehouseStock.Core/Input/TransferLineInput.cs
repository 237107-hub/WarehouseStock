using System.Globalization;
using WarehouseStock.Core.Domain;
using WarehouseStock.Core.Errors;

namespace WarehouseStock.Core.Input;

/// <summary>Перевіряє зовнішні поля перед створенням позиції.</summary>
public static class TransferLineInput
{
    /// <summary>Максимальна довжина артикула.</summary>
    public const int MaximumSkuLength = 20;

    /// <summary>Максимальна кількість в одному рядку.</summary>
    public const int MaximumQuantity = 1000;

    /// <summary>Верхня межа ціни захищає добуток від переповнення.</summary>
    public const decimal MaximumUnitPrice = 1_000_000_000m;

    private const NumberStyles PriceStyles = NumberStyles.AllowLeadingSign
        | NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite;

    /// <summary>Повертає позицію або повідомлення для повторного вводу.</summary>
    /// <param name="sku">Артикул довжиною до 20 символів.</param>
    /// <param name="quantity">Ціла кількість від 1 до 1000.</param>
    /// <param name="price">Ціна з десятковою крапкою без роздільників тисяч.</param>
    /// <returns>Перевірений результат без винятків для звичайних помилок вводу.</returns>
    public static Result<TransferLine> ParseFields(string? sku, string? quantity, string? price)
    {
        if (string.IsNullOrWhiteSpace(sku) || sku.Trim().Length > MaximumSkuLength)
        {
            return ResultFactory.Fail<TransferLine>("Артикул порожній або довший за 20 символів");
        }

        if (!int.TryParse(quantity, NumberStyles.Integer, CultureInfo.InvariantCulture, out int q)
            || q is < 1 or > MaximumQuantity)
        {
            return ResultFactory.Fail<TransferLine>("Кількість — ціле число більше 0 і не більше 1000");
        }

        if (!decimal.TryParse(price, PriceStyles, CultureInfo.InvariantCulture, out decimal p)
            || p is < 0m or > MaximumUnitPrice)
        {
            return ResultFactory.Fail<TransferLine>("Ціна — невід’ємне число з крапкою до 1000000000");
        }

        return ResultFactory.Ok(new TransferLine(sku.Trim(), q, p));
    }
}
