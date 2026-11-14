using WarehouseStock.Core.Domain;

namespace WarehouseStock.Core.Abstractions;

/// <summary>Незалежний від сховища контракт сповіщення.</summary>
public interface INotifier
{
    /// <summary>Повідомляє про зареєстроване переміщення.</summary>
    /// <param name="transfer">Документ, збережений сервісом.</param>
    void Notify(Transfer transfer);
}
