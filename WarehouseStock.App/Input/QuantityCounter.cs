using System.Globalization;

namespace WarehouseStock.App.Input;

/// <summary>Підрахунок коректних рядків без винятків як керування потоком.</summary>
public static class QuantityCounter
{
    /// <summary>Рахує рядки, що містять додатну кількість Int32.</summary>
    /// <param name="rows">Зовнішні рядки для перевірки.</param>
    /// <returns>Кількість коректних рядків, а не код помилки.</returns>
    public static int CountValidQuantities(IEnumerable<string?> rows)
    {
        ArgumentNullException.ThrowIfNull(rows);
        int valid = 0;
        foreach (string? row in rows)
        {
            if (int.TryParse(row, NumberStyles.Integer, CultureInfo.InvariantCulture, out int quantity)
                && quantity > 0)
            {
                valid++;
            }
        }

        return valid;
    }
}
