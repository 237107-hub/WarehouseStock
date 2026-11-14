using System.Text.Json;

namespace WarehouseStock.Tests.Legacy;

// Адаптер потрібен лише для компіляції навчального класу-Бога.
// Перевірки не викликають його метод і не створюють файлів.
public sealed class FileTransferStore
{
    private readonly JsonSerializerOptions _options = new() { WriteIndented = true };

    public void SaveToFile(LegacyTransfer transfer, string path)
    {
        File.WriteAllText(path, JsonSerializer.Serialize(transfer, _options));
    }
}
