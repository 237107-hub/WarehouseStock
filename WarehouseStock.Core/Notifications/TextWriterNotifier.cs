using System.Globalization;
using WarehouseStock.Core.Abstractions;
using WarehouseStock.Core.Domain;

namespace WarehouseStock.Core.Notifications;

/// <summary>Локальний демонстраційний канал без SMTP-відправки.</summary>
public sealed class TextWriterNotifier : INotifier
{
    private readonly TextWriter _output;

    /// <summary>Приймає вихідний канал без прив’язки до консолі.</summary>
    /// <param name="output">Незалежно керований текстовий приймач.</param>
    public TextWriterNotifier(TextWriter output)
    {
        ArgumentNullException.ThrowIfNull(output);
        _output = output;
    }

    /// <inheritdoc/>
    public void Notify(Transfer transfer)
    {
        ArgumentNullException.ThrowIfNull(transfer);
        _output.Write("Зареєстровано переміщення № ");
        _output.WriteLine(transfer.Id.ToString(CultureInfo.InvariantCulture));
    }
}
