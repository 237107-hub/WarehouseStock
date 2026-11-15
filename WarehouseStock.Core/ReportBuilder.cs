using System.Text;

namespace WarehouseStock.Core;

/// <summary>Формує простий текстовий звіт із готових рядків.</summary>
public sealed class ReportBuilder
{
    /// <summary>Заголовок, що задається при створенні.</summary>
    public string Title { get; init; } = "Звіт";

    /// <summary>Об'єднує рядки без конкатенації в циклі.</summary>
    /// <param name="rows">Ненульова колекція рядків.</param>
    /// <returns>Текст із платформними переносами рядків.</returns>
    public string Build(IReadOnlyList<string> rows)
    {
        ArgumentNullException.ThrowIfNull(rows);
        var text = new StringBuilder();
        text.AppendLine(Title.ToUpperInvariant());
        foreach (string row in rows)
        {
            text.AppendLine(row);
        }

        return text.ToString();
    }
}
