using System.Globalization;
using WarehouseStock.Core.Domain;
using WarehouseStock.Core.Services;

namespace WarehouseStock.App.Infrastructure;

/// <summary>Зовнішня операція запису звіту з володінням потоком через using.</summary>
public static class TransferReportExporter
{
    /// <summary>Закриває файл і при помилці розрахунку після відкриття.</summary>
    /// <param name="path">Шлях до вихідного файла.</param>
    /// <param name="transfer">Документ для звіту.</param>
    /// <param name="service">Сервіс облікової вартості.</param>
    public static void ExportReport(string path, Transfer transfer, TransferService service)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);
        ArgumentNullException.ThrowIfNull(transfer);
        ArgumentNullException.ThrowIfNull(service);
        using StreamWriter writer = new(path);
        writer.WriteLine("Документ № " + transfer.Id.ToString(CultureInfo.InvariantCulture));
        writer.WriteLine(service.TotalOf(transfer).ToString("F2", CultureInfo.InvariantCulture));
    }
}
