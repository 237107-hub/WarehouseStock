using WarehouseStock.Core.Abstractions;
using WarehouseStock.Core.Domain;
using WarehouseStock.Core.Errors;

namespace WarehouseStock.Core.Services;

/// <summary>Прикладний сценарій реєстрації та оцінювання переміщень.</summary>
public sealed class TransferService
{
    private readonly ITransferRepository _repository;
    private readonly IPricingPolicy _pricing;
    private readonly INotifier _notifier;

    /// <summary>Приймає всі зовнішні залежності як абстракції.</summary>
    /// <param name="repository">Сховище документів.</param>
    /// <param name="pricing">Стратегія облікової вартості.</param>
    /// <param name="notifier">Канал повідомлення про реєстрацію.</param>
    public TransferService(ITransferRepository repository, IPricingPolicy pricing, INotifier notifier)
    {
        ArgumentNullException.ThrowIfNull(repository);
        ArgumentNullException.ThrowIfNull(pricing);
        ArgumentNullException.ThrowIfNull(notifier);
        _repository = repository;
        _pricing = pricing;
        _notifier = notifier;
    }

    /// <summary>Зберігає документ і повідомляє про його реєстрацію.</summary>
    /// <param name="transfer">Документ зі вже захищеними інваріантами.</param>
    public void Place(Transfer transfer)
    {
        ArgumentNullException.ThrowIfNull(transfer);
        _repository.Add(transfer);
        _notifier.Notify(transfer);
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
            total += _pricing.PriceOf(line);
        }

        return total;
    }

    /// <summary>Завантажує обов’язковий документ замість поширення null.</summary>
    /// <param name="transferId">Додатний номер переміщення.</param>
    /// <returns>Знайдений документ або виняток transfer.exists.</returns>
    public Transfer Load(int transferId)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(transferId);
        return _repository.GetById(transferId) ?? throw new DomainRuleException(
            "transfer.exists", $"Переміщення {transferId} не знайдено.");
    }

    /// <summary>Приймає позицію, вже перевірену барикадою.</summary>
    /// <param name="transferId">Номер документа для редагування.</param>
    /// <param name="line">Типізована позиція без рядкового розбору.</param>
    public void AddLine(int transferId, TransferLine line)
    {
        ArgumentNullException.ThrowIfNull(line);
        Transfer transfer = Load(transferId);
        if (transfer.Status != TransferStatus.New)
        {
            throw new DomainRuleException(
                "transfer.editable",
                $"Переміщення {transferId} у стані {transfer.Status} редагувати не можна.");
        }

        transfer.AddLine(line);
    }

    /// <summary>Погоджує документ, якщо ліміт покриває облікову вартість.</summary>
    /// <param name="transferId">Номер нового переміщення.</param>
    /// <param name="allowedValue">Невід’ємний погоджений ліміт.</param>
    public void Approve(int transferId, decimal allowedValue)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(allowedValue);
        Transfer transfer = Load(transferId);
        EnsureApprovable(transfer);
        decimal required = TotalOf(transfer);
        EnsureLimit(transferId, allowedValue, required);
        transfer.Approve();
    }

    /// <summary>Скасовує документ з поясненою причиною.</summary>
    /// <param name="transferId">Номер документа.</param>
    /// <param name="reason">Непорожня причина скасування.</param>
    public void Cancel(int transferId, string reason)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(reason);
        Transfer transfer = Load(transferId);
        if (transfer.Status is not (TransferStatus.New or TransferStatus.Approved))
        {
            throw new DomainRuleException(
                "transfer.cancellable",
                $"Переміщення {transferId} у стані {transfer.Status} скасувати не можна.");
        }

        transfer.Cancel();
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
