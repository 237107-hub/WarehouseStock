using WarehouseStock.Core.Documents;

namespace WarehouseStock.Core.Legacy;

/// <summary>Еталонний довгий розрахунок для характеризаційних тестів.</summary>
public static class LegacyProcessor
{
    /// <summary>Перевіряє, рахує, змінює код стану та за потреби друкує документ.</summary>
    /// <param name="docId">Номер документа.</param>
    /// <param name="clientName">Назва одержувача.</param>
    /// <param name="clientEmail">Пошта одержувача.</param>
    /// <param name="isRegular">Ознака постійного одержувача.</param>
    /// <param name="lines">Позиції переміщення.</param>
    /// <param name="createdAt">Дата створення.</param>
    /// <param name="couponCode">Код знижки.</param>
    /// <param name="status">Початковий стан.</param>
    /// <param name="printToConsole">Ознака друку.</param>
    /// <param name="deliveryPrice">Ціна доставки.</param>
    /// <param name="newStatus">Код наступного стану.</param>
    /// <returns>Підсумок або мінус один для некоректного входу.</returns>
    public static decimal Process(
        int docId, string clientName, string clientEmail,
        bool isRegular, List<TransferLine> lines,
        DateOnly createdAt, string couponCode, string status,
        bool printToConsole, decimal deliveryPrice,
        out string newStatus)
    {
        newStatus = status;
        decimal total = 0;
        DocumentGuards.EnsureHeaderValid(docId, clientName, clientEmail);
        DocumentGuards.EnsureLinesValid(lines);
        if (status != "New" && status != "Approved"
            && status != "Dispatched" && status != "Cancelled")
        {
            Console.WriteLine("Помилка: невідомий стан");
            return -1;
        }

        for (int i = 0; i < lines.Count; i++)
        {
            decimal sum = lines[i].Quantity * lines[i].UnitPrice;
            if (lines[i].Quantity >= 10)
                sum = sum * 0.95m;
            total = total + sum;
        }

        if (isRegular)
            total = total * 0.93m;
        if (total > 5000)
            total = total * 0.9m;
        else if (total > 2000)
            total = total * 0.95m;
        if (couponCode == "SALE10")
            total = total - (total * 0.1m);
        else if (couponCode == "MINUS200" && total > 1000)
            total = total - 200;
        if (createdAt.DayOfWeek == DayOfWeek.Sunday)
            total = total * 0.98m;
        if (total < 1000)
            total = total + deliveryPrice;
        if (total < 0)
            total = 0;
        total = Math.Round(total, 2);
        if (status == "New" && total > 0)
            newStatus = "Approved";
        else if (status == "Approved")
            newStatus = "Dispatched";
        else if (status == "Cancelled")
        {
            newStatus = "Cancelled";
            total = 0;
        }

        if (printToConsole)
        {
            Console.WriteLine("Документ № " + docId);
            Console.WriteLine("Клієнт: " + clientName);
            Console.WriteLine("Пошта: " + clientEmail);
            for (int i = 0; i < lines.Count; i++)
            {
                Console.WriteLine(lines[i].Sku + " x " + lines[i].Quantity);
            }

            Console.WriteLine("Разом: " + total);
            Console.WriteLine("Стан: " + newStatus);
        }

        return total;
    }
}
