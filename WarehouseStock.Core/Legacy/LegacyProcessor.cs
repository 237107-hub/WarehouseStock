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
        DocumentGuards.EnsureHeaderValid(docId, clientName, clientEmail);
        DocumentStatus current = Enum.Parse<DocumentStatus>(status);
        decimal total = DocumentTotalCalculator.SubtotalOf(lines);
        total -= DocumentTotalCalculator.ClientDiscountOf(total, isRegular);
        total -= DocumentTotalCalculator.VolumeDiscountOf(total);
        total -= DocumentTotalCalculator.CouponDiscountOf(total, couponCode);
        total -= DocumentTotalCalculator.WeekendDiscountOf(total, createdAt);
        total += DocumentTotalCalculator.DeliveryFeeOf(total, deliveryPrice);
        total = current == DocumentStatus.Cancelled ? 0m : Math.Round(Math.Max(total, 0m), 2);
        newStatus = DocumentTotalCalculator.NextStatusOf(current, total).ToString();
        if (printToConsole)
        {
            Console.WriteLine(total);
        }

        return total;
    }
}
