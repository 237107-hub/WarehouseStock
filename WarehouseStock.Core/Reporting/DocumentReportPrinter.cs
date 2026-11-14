namespace WarehouseStock.Core.Reporting;

/// <summary>Команда виведення до явно переданого текстового приймача.</summary>
public sealed class DocumentReportPrinter
{
    private readonly TextWriter _output;

    /// <summary>Створює принтер для заданого приймача.</summary>
    /// <param name="output">Консоль, файл або буфер; власник керує його життєвим циклом.</param>
    public DocumentReportPrinter(TextWriter output)
    {
        ArgumentNullException.ThrowIfNull(output);
        _output = output;
    }

    /// <summary>Друкує готовий текст без обчислення значень.</summary>
    /// <param name="text">Непорожній текст звіту.</param>
    public void Print(string text)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(text);
        _output.Write(text);
    }
}
