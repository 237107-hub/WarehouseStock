using WarehouseStock.Core.Abstractions;
using WarehouseStock.Core.Domain;

namespace WarehouseStock.App.Infrastructure;

/// <summary>Явна демонстраційна ін’єкція одного збою; не справжній файловий адаптер.</summary>
public sealed class DemoTransferRepository : ITransferRepository
{
    private readonly ITransferRepository _inner;
    private bool _failNextRead;

    /// <summary>Обгортає справжнє навчальне сховище.</summary>
    /// <param name="inner">Сховище, якому делегуються успішні операції.</param>
    public DemoTransferRepository(ITransferRepository inner)
    {
        ArgumentNullException.ThrowIfNull(inner);
        _inner = inner;
    }

    /// <summary>Вмикає одноразовий штучний збій для перевірки межі.</summary>
    public void FailNextRead() => _failNextRead = true;

    /// <inheritdoc/>
    public void Add(Transfer transfer) => _inner.Add(transfer);

    /// <inheritdoc/>
    public Transfer? GetById(int id)
    {
        if (_failNextRead)
        {
            _failNextRead = false;
            throw new IOException("Навчальна імітація тимчасової недоступності сховища.");
        }

        return _inner.GetById(id);
    }

    /// <inheritdoc/>
    public IReadOnlyList<Transfer> GetAll() => _inner.GetAll();
}
