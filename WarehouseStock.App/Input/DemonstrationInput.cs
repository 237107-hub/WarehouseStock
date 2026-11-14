namespace WarehouseStock.App.Input;

/// <summary>Сценарний ввід з видимим повторенням реально прочитаних рядків.</summary>
public sealed class DemonstrationInput : TextReader
{
    private readonly StringReader _input;
    private readonly TextWriter _output;

    /// <summary>Створює повторюваний сценарій помилки та виправлення.</summary>
    /// <param name="output">Канал демонстрації вводу.</param>
    public DemonstrationInput(TextWriter output)
    {
        ArgumentNullException.ThrowIfNull(output);
        _input = new StringReader("SKU-1\nдві\n100\nSKU-1\n2\n100\n");
        _output = output;
    }

    /// <inheritdoc/>
    public override string? ReadLine()
    {
        string? line = _input.ReadLine();
        if (line is not null)
        {
            _output.WriteLine(line);
        }

        return line;
    }

    /// <inheritdoc/>
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _input.Dispose();
        }

        base.Dispose(disposing);
    }
}
