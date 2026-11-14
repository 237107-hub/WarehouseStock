namespace WarehouseStock.Core.Errors;

/// <summary>Порушення іменованого правила предметної області.</summary>
public sealed class DomainRuleException : Exception
{
    /// <summary>Створює помилку правила з контекстним повідомленням.</summary>
    /// <param name="rule">Непорожній ідентифікатор правила.</param>
    /// <param name="message">Пояснення українською з конкретними значеннями.</param>
    public DomainRuleException(string rule, string message)
        : base(message)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(rule);
        ArgumentException.ThrowIfNullOrWhiteSpace(message);
        Rule = rule;
    }

    /// <summary>Зберігає також причину нижчого рівня.</summary>
    /// <param name="rule">Ідентифікатор порушеної умови.</param>
    /// <param name="message">Зрозуміле повідомлення з контекстом операції.</param>
    /// <param name="innerException">Початковий виняток зі своїм стеком.</param>
    public DomainRuleException(string rule, string message, Exception innerException)
        : base(message, innerException)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(rule);
        ArgumentException.ThrowIfNullOrWhiteSpace(message);
        ArgumentNullException.ThrowIfNull(innerException);
        Rule = rule;
    }

    /// <summary>Ідентифікатор правила для перевірки та структурованого журналу.</summary>
    public string Rule { get; }
}
