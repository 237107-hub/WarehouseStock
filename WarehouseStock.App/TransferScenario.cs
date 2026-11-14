using System.Globalization;
using WarehouseStock.App.Input;
using WarehouseStock.Core.Domain;
using WarehouseStock.Core.Errors;
using WarehouseStock.Core.Services;

namespace WarehouseStock.App;

/// <summary>Зовнішній сценарій вводу й одноразового повтору збою середовища.</summary>
public sealed class TransferScenario
{
    private readonly TransferService _service;
    private readonly TextWriter _output;
    private readonly Action _simulateFailure;

    /// <summary>Приймає готовий сервіс та контрольовані канали взаємодії.</summary>
    /// <param name="service">Типізоване ядро сценарію.</param>
    /// <param name="output">Вихід для пояснень користувачеві.</param>
    /// <param name="simulateFailure">Явний сценарний гачок, порожній у звичайному вводі.</param>
    public TransferScenario(TransferService service, TextWriter output, Action simulateFailure)
    {
        ArgumentNullException.ThrowIfNull(service);
        ArgumentNullException.ThrowIfNull(output);
        ArgumentNullException.ThrowIfNull(simulateFailure);
        _service = service;
        _output = output;
        _simulateFailure = simulateFailure;
    }

    /// <summary>Виконує ввід, розрахунок і погодження одного документа.</summary>
    /// <param name="input">Сценарний або справжній консольний ввід.</param>
    /// <returns>0 при успіху або 1 після завершення вводу без позиції.</returns>
    public int Run(TextReader input)
    {
        ArgumentNullException.ThrowIfNull(input);
        Transfer transfer = new(1, 10, 20, new DateOnly(2026, 11, 14));
        _service.Place(transfer);
        Result<TransferLine> result = LineInput.ReadValidLine(input, _output);
        if (!result.IsSuccess)
        {
            _output.WriteLine(result.Error);
            return 1;
        }

        _service.AddLine(transfer.Id, result.Value);
        _simulateFailure();
        ApproveWithRetry(transfer.Id);
        _output.WriteLine("Сума: " + _service.TotalOf(transfer).ToString("F2", CultureInfo.InvariantCulture));
        return 0;
    }

    private void ApproveWithRetry(int id)
    {
        try
        {
            _service.Approve(id, 200m);
        }
        catch (IOException)
        {
            _output.WriteLine("Сховище тимчасово недоступне; повторюю операцію один раз.");
            _service.Approve(id, 200m);
        }
    }
}
