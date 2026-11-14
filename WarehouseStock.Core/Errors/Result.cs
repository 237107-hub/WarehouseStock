namespace WarehouseStock.Core.Errors;

/// <summary>Очікуваний результат вводу зі захищеним доступом до значення.</summary>
/// <typeparam name="T">Ненульовий тип успішного результату.</typeparam>
public readonly struct Result<T>
    where T : notnull
{
    private readonly T? _value;
    private readonly string? _error;

    /// <summary>Внутрішній конструктор для перевірених фабрик Result.</summary>
    /// <param name="isSuccess">Ознака успіху.</param>
    /// <param name="value">Значення для успішного результату.</param>
    /// <param name="error">Повідомлення для невдачі.</param>
    internal Result(bool isSuccess, T? value, string error)
    {
        IsSuccess = isSuccess;
        _value = value;
        _error = error;
    }

    /// <summary>Ознака наявності перевіреного значення.</summary>
    public bool IsSuccess { get; }

    /// <summary>Значення лише успішного результату; інакше порушено контракт виклику.</summary>
    public T Value => IsSuccess ? _value! : throw new InvalidOperationException(Error);

    /// <summary>Пояснення невдачі, включно з default-результатом.</summary>
    public string Error => IsSuccess ? string.Empty : _error ?? "Результат не ініціалізований.";
}
