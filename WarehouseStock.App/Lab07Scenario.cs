using System.Globalization;
using WarehouseStock.Core.Pricing;

namespace WarehouseStock.App;

/// <summary>Окрема демонстрація правила, розробленого через TDD.</summary>
internal static class Lab07Scenario
{
    /// <summary>Друкує приклади без доступу до сховища та мережі.</summary>
    /// <param name="output">Потік демонстрації.</param>
    internal static void Run(TextWriter output)
    {
        output.WriteLine("ЛР7 / WarehouseStock / варіант 6");
        output.WriteLine("Знижка постійному одержувачу: 5% + 10% від 1000; стеля 150 грн.");
        foreach (decimal amount in new[] { 0m, 400m, 999.99m, 1000m, 1000.04m, 5000m })
        {
            decimal discount = TransferDiscountPolicy.DiscountFor(amount, true);
            output.WriteLine(amount.ToString("F2", CultureInfo.InvariantCulture) + " -> " +
                discount.ToString("F2", CultureInfo.InvariantCulture) + " UAH");
        }

        output.WriteLine("Тести: dotnet test. Звіт покриття: Coverage/after/index.html.");
    }
}
