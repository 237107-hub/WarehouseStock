namespace WarehouseStock.Core.Documents;

/// <summary>Перевірки контрактів перед виконанням розрахунків.</summary>
internal static class DocumentGuards
{
    /// <summary>Перевіряє заголовок документа.</summary>
    /// <param name="documentId">Додатний номер документа.</param>
    /// <param name="recipientName">Непорожня назва одержувача.</param>
    /// <param name="recipientEmail">Адреса пошти одержувача.</param>
    public static void EnsureHeaderValid(int documentId, string recipientName, string recipientEmail)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(documentId);
        ArgumentException.ThrowIfNullOrWhiteSpace(recipientName);
        ArgumentException.ThrowIfNullOrWhiteSpace(recipientEmail);
        if (!recipientEmail.Contains('@'))
        {
            throw new ArgumentException("Пошта має містити символ @.", nameof(recipientEmail));
        }
    }

    /// <summary>Перевіряє позицію документа.</summary>
    /// <param name="line">Значення для перевірки.</param>
    public static void EnsureLineValid(TransferLine line)
    {
        ArgumentNullException.ThrowIfNull(line);
        ArgumentException.ThrowIfNullOrWhiteSpace(line.Sku);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(line.Quantity, nameof(line.Quantity));
        ArgumentOutOfRangeException.ThrowIfNegative(line.UnitPrice, nameof(line.UnitPrice));
    }

    /// <summary>Перевіряє колекцію позицій.</summary>
    /// <param name="lines">Значення для перевірки.</param>
    public static void EnsureLinesValid(IReadOnlyList<TransferLine> lines)
    {
        ArgumentNullException.ThrowIfNull(lines);
        if (lines.Count == 0)
        {
            throw new ArgumentException("Документ не містить жодної позиції.", nameof(lines));
        }

        foreach (TransferLine line in lines)
        {
            EnsureLineValid(line);
        }
    }

    /// <summary>Відхиляє числові значення поза переліком станів.</summary>
    /// <param name="status">Стан для перевірки.</param>
    public static void EnsureStatusValid(DocumentStatus status)
    {
        if (!Enum.IsDefined(status))
        {
            throw new ArgumentOutOfRangeException(nameof(status), status, "Невідомий стан документа.");
        }
    }
}
