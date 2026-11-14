using Microsoft.Extensions.Logging;

namespace WarehouseStock.App.Diagnostics;

/// <summary>Згенеровані структуровані події без вимкнення CA1848.</summary>
internal static partial class ApplicationLog
{
    /// <summary>Записує подію Rule.</summary>
    /// <param name="logger">Структурований журнал.</param>
    /// <param name="rule">Ідентифікатор правила.</param>
    /// <param name="detail">Пояснення з контекстом.</param>
    [LoggerMessage(EventId = 1, Level = LogLevel.Warning, Message = "Правило {Rule}: {Detail}")]
    internal static partial void Rule(ILogger logger, string rule, string detail);

    /// <summary>Записує подію IoFailure.</summary>
    /// <param name="logger">Структурований журнал.</param>
    /// <param name="exception">Помилка вводу-виводу зі стеком.</param>
    [LoggerMessage(EventId = 2, Level = LogLevel.Error, Message = "Операція вводу-виводу не завершена")]
    internal static partial void IoFailure(ILogger logger, Exception exception);

    /// <summary>Записує подію AccessDenied.</summary>
    /// <param name="logger">Структурований журнал.</param>
    /// <param name="exception">Початкова помилка доступу.</param>
    [LoggerMessage(EventId = 3, Level = LogLevel.Error, Message = "Немає прав на запис")]
    internal static partial void AccessDenied(ILogger logger, Exception exception);

    /// <summary>Записує подію Unexpected.</summary>
    /// <param name="logger">Структурований журнал.</param>
    /// <param name="exception">Неперехоплений внутрішній дефект.</param>
    [LoggerMessage(EventId = 4, Level = LogLevel.Critical, Message = "Непередбачений збій застосунку")]
    internal static partial void Unexpected(ILogger logger, Exception exception);
}
