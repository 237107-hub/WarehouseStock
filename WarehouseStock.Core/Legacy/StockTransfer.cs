using System;
using System.Collections.Generic;
using System.Linq;

namespace WarehouseStock.Core.Legacy;

/// <summary>Успадкований документ переміщення з розрахунком вартості.</summary>
public class StockTransfer
{
    private readonly List<string[]> _lines = new List<string[]>();

    /// <summary>Створює документ для зазначеного одержувача.</summary>
    /// <param name="id">Ідентифікатор документа.</param>
    /// <param name="recipientName">Назва одержувача товарів.</param>
    public StockTransfer(string id, string recipientName)
    {
        Id = id;
        RecipientName = recipientName;
        CreatedAt = DateTime.Now;
    }

    /// <summary>Ідентифікатор документа.</summary>
    public string Id { get; private set; }

    /// <summary>Назва одержувача товарів.</summary>
    public string RecipientName { get; private set; }

    /// <summary>Стан: 0 — новий, 1 — погоджений, 2 — відправлений, 3 — скасований.</summary>
    public int Status { get; private set; } = 0;

    /// <summary>Локальний час створення документа.</summary>
    public DateTime CreatedAt { get; private set; }

    /// <summary>Знаходить перший документ із заданим ідентифікатором.</summary>
    /// <param name="transfers">Список документів для пошуку.</param>
    /// <param name="transferId">Ідентифікатор потрібного документа.</param>
    /// <returns>Знайдений документ або null, якщо збігів немає.</returns>
    public static StockTransfer? FindById(List<StockTransfer> transfers, string transferId)
    {
        for (int i = 0; i < transfers.Count; i++)
        {
            if (transfers[i].Id == transferId)
            {
                return transfers[i];
            }
        }

        return null;
    }

    /// <summary>Додає позицію товару до документа.</summary>
    /// <param name="sku">Артикул товару.</param>
    /// <param name="quantity">Кількість одиниць товару.</param>
    /// <param name="unitPrice">Облікова ціна одиниці товару.</param>
    public void AddLine(string sku, int quantity, decimal unitPrice)
    {
        string[] line = new string[3];
        line[0] = sku;
        line[1] = quantity.ToString();
        line[2] = unitPrice.ToString();
        _lines.Add(line);
    }

    /// <summary>Обчислює суму з успадкованими знижками та ПДВ.</summary>
    /// <param name="isRegularRecipient">Ознака одержувача з регулярною знижкою.</param>
    /// <returns>Сума після знижок і ПДВ, округлена до двох десяткових знаків.</returns>
    public decimal CalculateTotal(bool isRegularRecipient)
    {
        decimal total = 0;
        int lineCount = 0;

        for (int i = 0; i < _lines.Count; i++)
        {
            int quantity = int.Parse(_lines[i][1]);
            decimal unitPrice = decimal.Parse(_lines[i][2]);
            total = total + (quantity * unitPrice);
            lineCount = lineCount + 1;
        }

        // Порядок знижок успадковано; пільги не підсумовуються.
        if (isRegularRecipient == true && total > 1000)
        {
            total = total * 0.9m;
        }
        else if (total > 5000)
        {
            total = total * 0.85m;
        }
        else
        {
            total = total;
        }

        if (lineCount > 10)
        {
            total = total - 100;
        }

        if (total < 0)
        {
            total = 0;
        }

        // ПДВ нараховується після всіх знижок.
        total = total + (total * 0.2m);
        return Math.Round(total, 2);
    }

    /// <summary>Змінює стан лише для дозволеного успадкованого переходу.</summary>
    /// <param name="newStatus">Код цільового стану.</param>
    /// <returns>True, якщо перехід виконано; false — якщо заборонено.
    /// У разі false поточний стан не змінюється.</returns>
    public bool TryChangeStatus(int newStatus)
    {
        if (Status == 0 && newStatus == 1)
        {
            Status = 1;
            return true;
        }

        if (Status == 1 && newStatus == 2)
        {
            Status = 2;
            return true;
        }

        if (Status == 0 && newStatus == 3)
        {
            Status = 3;
            return true;
        }

        return false;
    }

    /// <summary>Перевіряє документ за успадкованими критеріями коректності.</summary>
    /// <returns>True, якщо всі критерії виконано; інакше false.</returns>
    public bool IsValid()
    {
        if (Id != null
            && Id != string.Empty
            && RecipientName != null
            && RecipientName.Length > 2
            && _lines.Count > 0
            && _lines.Count < 100
            && Status >= 0
            && Status <= 3)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    /// <summary>Формує текстовий звіт із позиціями та загальною сумою.</summary>
    /// <returns>Звіт із підсумком без регулярної знижки.</returns>
    public string BuildReport()
    {
        string report = string.Empty;

        for (int i = 0; i < _lines.Count; i++)
        {
            report = report
                + "Товар: " + _lines[i][0]
                + "; кількість: " + _lines[i][1]
                + "; ціна: " + _lines[i][2]
                + "; сума: " + (int.Parse(_lines[i][1]) * decimal.Parse(_lines[i][2]))
                + "\n";
        }

        report = report + "Разом: " + CalculateTotal(false) + "\n";
        return report;
    }
}
