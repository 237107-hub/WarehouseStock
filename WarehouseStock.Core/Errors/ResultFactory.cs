namespace WarehouseStock.Core.Errors;

/// <summary>Неузагальнені фабрики результатів без вимкнення правила CA1000.</summary>
public static class ResultFactory
{
    /// <summary>Формує успішний результат без null.</summary>
    /// <typeparam name="T">Тип перевіреного значення.</typeparam>
    /// <param name="value">Ненульове значення.</param>
    /// <returns>Успішний результат.</returns>
    public static Result<T> Ok<T>(T value)
        where T : notnull
    {
        ArgumentNullException.ThrowIfNull(value);
        return new Result<T>(true, value, string.Empty);
    }

    /// <summary>Формує очікувану невдачу з поясненням.</summary>
    /// <typeparam name="T">Тип відсутнього значення.</typeparam>
    /// <param name="error">Непорожнє повідомлення.</param>
    /// <returns>Невдалий результат.</returns>
    public static Result<T> Fail<T>(string error)
        where T : notnull
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(error);
        return new Result<T>(false, default, error);
    }
}
