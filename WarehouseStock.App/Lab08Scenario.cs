using System.Globalization;
using WarehouseStock.Core.Input;

namespace WarehouseStock.App;

/// <summary>Показує версію та перевірку меж без файлів і мережі.</summary>
internal static class Lab08Scenario
{
    /// <summary>Друкує прийняті й відхилені приклади.</summary>
    /// <param name="output">Потік демонстрації.</param>
    internal static void Run(TextWriter output)
    {
        output.WriteLine("ЛР8 / WarehouseStock / варіант 6");
        foreach (string raw in new[] { "SKU-1;2;10.50", "SKU-1;0;10.50", "SKU-1;1001;10.50", "SKU-1;2;10,50" })
        {
            bool ok = TransferCsvLineParser.TryParse(raw, out var line);
            output.WriteLine(raw + " -> " + (ok ? "OK " + line!.Amount.ToString("F2", CultureInfo.InvariantCulture) : "REJECT"));
        }

        output.WriteLine("Локальні перевірки: eng/quality.ps1. GitHub Actions потребує публікації репозиторію.");
    }
}
