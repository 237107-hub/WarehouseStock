using Microsoft.Extensions.Logging;
using WarehouseStock.Core.Abstractions;
using WarehouseStock.Core.Diagnostics;
using WarehouseStock.Core.Domain;
using WarehouseStock.Core.Errors;

namespace WarehouseStock.Core.Services;

/// <summary>Прикладний сценарій реєстрації та оцінювання переміщень.</summary>
public sealed class TransferService
{
    private readonly ITransferRepository _repository;
    private readonly IPricingPolicy _pricing;
    private readonly INotifier _notifier;
    private readonly ILogger<TransferService> _log;

    /// <summary>Приймає всі зовнішні залежності як абстракції.</summary>
    /// <param name="repository">Сховище документів.</param>
    /// <param name="pricing">Стратегія облікової вартості.</param>
    /// <param name="notifier">Канал повідомлення про реєстрацію.</param>
    /// <param name="log">Структурований журнал операцій.</param>
    public TransferService(
        ITransferRepository repository,
        IPricingPolicy pricing,
        INotifier notifier,
        ILogger<TransferService> log)
    {
        ArgumentNullException.ThrowIfNull(repository);
        ArgumentNullException.ThrowIfNull(pricing);
        ArgumentNullException.ThrowIfNull(notifier);
        ArgumentNullException.ThrowIfNull(log);
        _log = log;
        _repository = repository;
        _pricing = pricing;
        _notifier = notifier;
    }

    /// <summary>Зберігає документ і повідомляє про його реєстрацію.</summary>
    /// <param name="transfer">Документ зі вже захищеними інваріантами.</param>
    public void Place(Transfer transfer)
    {
        ArgumentNullException.ThrowIfNull(transfer);
        using IDisposable? scope = TransferLog.Scope(_log, transfer.Id);
        TransferLog.RegistrationStarted(_log, transfer.Id);
        _repository.Add(transfer);
        _notifier.Notify(transfer);
        TransferLog.Registered(_log, transfer.Id);
    }

    /// <summary>Обчислює суму без збереження та сповіщення.</summary>
    /// <param name="transfer">Джерело позицій.</param>
    /// <returns>Сума вартостей за переданою політикою.</returns>
    public decimal TotalOf(Transfer transfer)
    {
        ArgumentNullException.ThrowIfNull(transfer);
        decimal total = 0m;
        foreach (TransferLine line in transfer.Lines)
        {
            decimal value = _pricing.PriceOf(line);
            if (value < 0m)
            {
                throw new InvalidOperationException("CALC-02: політика повернула від’ємну вартість.");
            }

            total += value;
        }

        return total;
    }

    /// <summary>Завантажує обов’язковий документ замість поширення null.</summary>
    /// <param name="transferId">Додатний номер переміщення.</param>
    /// <returns>Знайдений документ або виняток transfer.exists.</returns>
    public Transfer Load(int transferId)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(transferId);
        using IDisposable? scope = TransferLog.Scope(_log, transferId);
        try
        {
            return _repository.GetById(transferId) ?? throw new DomainRuleException(
                "transfer.exists", $"Переміщення {transferId} не знайдено.");
        }
        catch (IOException exception)
        {
            TransferLog.StorageFailure(_log, exception, transferId);
            throw;
        }
    }

    /// <summary>Приймає позицію, вже перевірену барикадою.</summary>
    /// <param name="transferId">Номер документа для редагування.</param>
    /// <param name="line">Типізована позиція без рядкового розбору.</param>
    public void AddLine(int transferId, TransferLine line)
    {
        ArgumentNullException.ThrowIfNull(line);
        using IDisposable? scope = TransferLog.Scope(_log, transferId);
        Transfer transfer = Load(transferId);
        if (transfer.Status != TransferStatus.New)
        {
            throw new DomainRuleException(
                "transfer.editable",
                $"Переміщення {transferId} у стані {transfer.Status} редагувати не можна.");
        }

        transfer.AddLine(line);
        TransferLog.LineAdded(_log, line.Sku, line.Quantity);
    }

    /// <summary>Погоджує документ, якщо ліміт покриває облікову вартість.</summary>
    /// <param name="transferId">Номер нового переміщення.</param>
    /// <param name="allowedValue">Невід’ємний погоджений ліміт.</param>
    public void Approve(int transferId, decimal allowedValue)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(allowedValue);
        using IDisposable? scope = TransferLog.Scope(_log, transferId);
        TransferLog.ApprovalStarted(_log, allowedValue);
        Transfer transfer = Load(transferId);
        EnsureApprovable(transfer);
        decimal required = TotalOf(transfer);
        EnsureLimit(transferId, allowedValue, required);
        if (allowedValue > required)
        {
            TransferLog.UnusedLimit(_log, allowedValue - required);
        }

        transfer.Approve();
        TransferLog.Approved(_log, transferId, required);
    }

    /// <summary>Скасовує документ з поясненою причиною.</summary>
    /// <param name="transferId">Номер документа.</param>
    /// <param name="reason">Непорожня причина скасування.</param>
    public void Cancel(int transferId, string reason)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(reason);
        using IDisposable? scope = TransferLog.Scope(_log, transferId);
        Transfer transfer = Load(transferId);
        if (transfer.Status is not (TransferStatus.New or TransferStatus.Approved))
        {
            throw new DomainRuleException(
                "transfer.cancellable",
                $"Переміщення {transferId} у стані {transfer.Status} скасувати не можна.");
        }

        transfer.Cancel();
        TransferLog.Cancelled(_log, transferId, reason);
    }

    private static void EnsureApprovable(Transfer transfer)
    {
        if (transfer.Status != TransferStatus.New)
        {
            throw new DomainRuleException(
                "transfer.approvable",
                $"Стан {transfer.Status} переміщення {transfer.Id} не дозволяє погодження.");
        }

        if (transfer.Lines.Count == 0)
        {
            throw new DomainRuleException("transfer.lines", $"Переміщення {transfer.Id} порожнє.");
        }
    }

    private static void EnsureLimit(int transferId, decimal allowedValue, decimal required)
    {
        if (allowedValue < required)
        {
            throw new DomainRuleException(
                "transfer.amount",
                $"Переміщення {transferId}: дозволено {allowedValue}, потрібно {required}.");
        }
    }
}
