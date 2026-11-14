using WarehouseStock.Core.Domain;
using WarehouseStock.Core.Errors;

namespace WarehouseStock.App.Input;

/// <summary>Повторний запит очікуваних помилок без аварійного завершення.</summary>
public static class LineInput
{
    /// <summary>Запитує рядок документа, доки дані коректні або ввід не завершено.</summary>
    /// <param name="input">Явно переданий текстовий ввід.</param>
    /// <param name="output">Канал підказок і пояснень.</param>
    /// <returns>Коректна позиція або повідомлення про кінець вводу.</returns>
    public static Result<TransferLine> ReadValidLine(TextReader input, TextWriter output)
    {
        ArgumentNullException.ThrowIfNull(input);
        ArgumentNullException.ThrowIfNull(output);
        while (true)
        {
            output.Write("Артикул: ");
            string? sku = input.ReadLine();
            if (sku is null)
            {
                return ResultFactory.Fail<TransferLine>("Ввід завершено.");
            }

            Result<TransferLine> result = ReadAttempt(sku, input, output);
            if (result.IsSuccess)
            {
                return result;
            }

            output.WriteLine("Помилка вводу: " + result.Error);
        }
    }

    private static Result<TransferLine> ReadAttempt(string sku, TextReader input, TextWriter output)
    {
        output.Write("Кількість: ");
        string? quantity = input.ReadLine();
        output.Write("Ціна (крапка): ");
        string? price = input.ReadLine();
        return TransferLineParser.Parse(sku, quantity, price);
    }
}
