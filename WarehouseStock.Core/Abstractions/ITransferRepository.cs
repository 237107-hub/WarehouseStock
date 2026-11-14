using WarehouseStock.Core.Domain;

namespace WarehouseStock.Core.Abstractions;

/// <summary>Вузький контракт зберігання документів переміщення.</summary>
public interface ITransferRepository
{
    /// <summary>Реєструє документ з новим унікальним номером.</summary>
    /// <param name="transfer">Переміщення для зберігання.</param>
    void Add(Transfer transfer);

    /// <summary>Шукає документ за номером.</summary>
    /// <param name="id">Додатний номер документа.</param>
    /// <returns>Знайдений документ або null.</returns>
    Transfer? GetById(int id);

    /// <summary>Повертає окрему колекцію посилань на збережені документи.</summary>
    /// <returns>Знімок складу сховища, а не глибока копія документів.</returns>
    IReadOnlyList<Transfer> GetAll();
}
