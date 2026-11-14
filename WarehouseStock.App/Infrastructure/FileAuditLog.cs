namespace WarehouseStock.App.Infrastructure;

/// <summary>Владелец файлового потоку аудиту з гарантованим звільненням.</summary>
public sealed class FileAuditLog : IDisposable
{
    private readonly StreamWriter _writer;
    private bool _disposed;

    /// <summary>Відкриває файл на дописування, не перезаписує історію.</summary>
    /// <param name="path">Шлях до локального файла аудиту.</param>
    public FileAuditLog(string path)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);
        _writer = new StreamWriter(path, append: true);
    }

    /// <summary>Записує час UTC, операцію та номер документа.</summary>
    /// <param name="operation">Одна операція без переносів рядка й роздільника ;.</param>
    /// <param name="documentId">Додатний номер документа.</param>
    public void Write(string operation, int documentId)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        ArgumentException.ThrowIfNullOrWhiteSpace(operation);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(documentId);
        if (operation.IndexOfAny(['\r', '\n', ';']) >= 0)
        {
            throw new ArgumentException("Операція аудиту має бути одним полем.", nameof(operation));
        }

        _writer.WriteLine(FormattableString.Invariant($"{DateTimeOffset.UtcNow:O};{operation};{documentId}"));
    }

    /// <summary>Закриває потік; повторне закриття безпечне.</summary>
    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        try
        {
            _writer.Dispose();
        }
        finally
        {
            _disposed = true;
            GC.SuppressFinalize(this);
        }
    }
}
