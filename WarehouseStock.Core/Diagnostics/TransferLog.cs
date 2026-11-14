using Microsoft.Extensions.Logging;

namespace WarehouseStock.Core.Diagnostics;

/// <summary>Згенеровані структуровані події без вимкнення CA1848.</summary>
internal static partial class TransferLog
{
    private static readonly Func<ILogger, int, IDisposable?> _scope =
        LoggerMessage.DefineScope<int>("Transfer:{TransferId}");

    /// <summary>Відкриває структурований контекст документа.</summary>
    /// <param name="logger">Журнал операції.</param>
    /// <param name="id">Ідентифікатор документа.</param>
    /// <returns>Область, яка закривається через using.</returns>
    internal static IDisposable? Scope(ILogger logger, int id) => _scope(logger, id);

    /// <summary>Записує подію RegistrationStarted.</summary>
    /// <param name="logger">Структурований журнал.</param>
    /// <param name="transferId">Номер документа.</param>
    [LoggerMessage(EventId = 1, Level = LogLevel.Debug, Message = "Початок реєстрації {TransferId}")]
    internal static partial void RegistrationStarted(ILogger logger, int transferId);

    /// <summary>Записує подію Registered.</summary>
    /// <param name="logger">Структурований журнал.</param>
    /// <param name="transferId">Номер створеного документа.</param>
    [LoggerMessage(EventId = 2, Level = LogLevel.Information, Message = "Переміщення {TransferId} зареєстровано")]
    internal static partial void Registered(ILogger logger, int transferId);

    /// <summary>Записує подію LineAdded.</summary>
    /// <param name="logger">Структурований журнал.</param>
    /// <param name="sku">Артикул позиції.</param>
    /// <param name="quantity">Кількість одиниць.</param>
    [LoggerMessage(EventId = 3, Level = LogLevel.Information, Message = "Додано {Sku}, кількість {Quantity}")]
    internal static partial void LineAdded(ILogger logger, string sku, int quantity);

    /// <summary>Записує подію ApprovalStarted.</summary>
    /// <param name="logger">Структурований журнал.</param>
    /// <param name="allowedValue">Дозволений ліміт вартості.</param>
    [LoggerMessage(EventId = 4, Level = LogLevel.Debug, Message = "Початок погодження, ліміт {AllowedValue}")]
    internal static partial void ApprovalStarted(ILogger logger, decimal allowedValue);

    /// <summary>Записує подію UnusedLimit.</summary>
    /// <param name="logger">Структурований журнал.</param>
    /// <param name="extra">Різниця між лімітом і вартістю.</param>
    [LoggerMessage(EventId = 5, Level = LogLevel.Warning, Message = "Невикористаний ліміт {Extra}")]
    internal static partial void UnusedLimit(ILogger logger, decimal extra);

    /// <summary>Записує подію Approved.</summary>
    /// <param name="logger">Структурований журнал.</param>
    /// <param name="transferId">Номер погодженого документа.</param>
    /// <param name="amount">Облікова вартість.</param>
    [LoggerMessage(EventId = 6, Level = LogLevel.Information, Message = "Погоджено {TransferId} на {Amount}")]
    internal static partial void Approved(ILogger logger, int transferId, decimal amount);

    /// <summary>Записує подію Cancelled.</summary>
    /// <param name="logger">Структурований журнал.</param>
    /// <param name="transferId">Номер скасованого документа.</param>
    /// <param name="reason">Причина скасування.</param>
    [LoggerMessage(EventId = 7, Level = LogLevel.Information, Message = "Скасовано {TransferId}, причина {Reason}")]
    internal static partial void Cancelled(ILogger logger, int transferId, string reason);

    /// <summary>Записує подію StorageFailure.</summary>
    /// <param name="logger">Структурований журнал.</param>
    /// <param name="exception">Початковий виняток зі стеком.</param>
    /// <param name="transferId">Номер запитаного документа.</param>
    [LoggerMessage(EventId = 8, Level = LogLevel.Error, Message = "Збій сховища для {TransferId}")]
    internal static partial void StorageFailure(ILogger logger, Exception exception, int transferId);
}
