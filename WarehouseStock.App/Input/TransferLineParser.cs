using WarehouseStock.Core.Domain;
using WarehouseStock.Core.Errors;
using WarehouseStock.Core.Input;

namespace WarehouseStock.App.Input;

/// <summary>Адаптер консольного вводу до спільної барикади ядра.</summary>
public static class TransferLineParser
{
    /// <summary>Перевіряє рядки без винятків для звичайних помилок користувача.</summary>
    /// <param name="sku">Артикул товару.</param>
    /// <param name="quantity">Ціла додатна кількість до 1000.</param>
    /// <param name="price">Невід’ємна ціна з десятковою крапкою.</param>
    /// <returns>Позиція або пояснення, яке можна показати й повторити ввід.</returns>
    public static Result<TransferLine> Parse(string? sku, string? quantity, string? price)
        => TransferLineInput.ParseFields(sku, quantity, price);
}
