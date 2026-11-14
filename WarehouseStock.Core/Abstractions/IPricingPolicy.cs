using WarehouseStock.Core.Domain;

namespace WarehouseStock.Core.Abstractions;

/// <summary>Одна стратегія визначення невід’ємної вартості позиції.</summary>
public interface IPricingPolicy
{
    /// <summary>Обчислює вартість позиції без її зміни.</summary>
    /// <param name="line">Перевірена незмінна позиція.</param>
    /// <returns>Невід’ємна вартість за обраною політикою.</returns>
    decimal PriceOf(TransferLine line);
}
