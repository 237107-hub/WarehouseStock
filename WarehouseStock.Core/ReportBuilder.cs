namespace WarehouseStock.Core;

/// <summary>Навмисно дефектний звіт для вправи статичного аналізу.</summary>
public class ReportBuilder
{
    /// <summary>Заголовок звіту.</summary>
    public string Title = "Звіт";

    /// <summary>Об'єднує рядки.</summary>
    /// <param name="rows">Рядки звіту.</param>
    /// <returns>Готовий текст.</returns>
    public string Build(List<string> rows)
    {
        string text = "";
        int notUsed = 0;
        foreach (var row in rows)
        {
            text = text + row + Environment.NewLine;
        }

        return Title.ToUpper() + Environment.NewLine + text;
    }
}
