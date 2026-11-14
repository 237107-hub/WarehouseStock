using Microsoft.Extensions.Logging;
using WarehouseStock.Core.Services;

namespace WarehouseStock.Tests;

internal sealed record CapturedLog(LogLevel Level, Exception? Exception,
    IReadOnlyDictionary<string, object?> Fields, IReadOnlyList<string> Scopes);

internal sealed class TestLogger : ILogger<TransferService>
{
    private readonly List<string> _scopes = new();

    public List<CapturedLog> Entries { get; } = new();

    public int ActiveScopes => _scopes.Count;

    public IDisposable BeginScope<TState>(TState state)
        where TState : notnull
    {
        _scopes.Add(state.ToString() ?? string.Empty);
        return new TestScope(() => _scopes.RemoveAt(_scopes.Count - 1));
    }

    public bool IsEnabled(LogLevel logLevel) => true;

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state,
        Exception? exception, Func<TState, Exception?, string> formatter)
    {
        Dictionary<string, object?> fields = state is IEnumerable<KeyValuePair<string, object?>> values
            ? values.ToDictionary(pair => pair.Key, pair => pair.Value, StringComparer.Ordinal)
            : new Dictionary<string, object?>(StringComparer.Ordinal);
        Entries.Add(new CapturedLog(logLevel, exception, fields, _scopes.ToArray()));
    }

    private sealed class TestScope(Action close) : IDisposable
    {
        private bool _disposed;

        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            _disposed = true;
            close();
            GC.SuppressFinalize(this);
        }
    }
}
