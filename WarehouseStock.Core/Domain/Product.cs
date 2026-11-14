namespace WarehouseStock.Core.Domain;

/// <summary>Товар із залишком, змінюваним тільки перевіреними операціями.</summary>
public sealed class Product
{
    /// <summary>Створює товар з невід’ємним залишком.</summary>
    /// <param name="sku">Унікальний у довіднику артикул.</param>
    /// <param name="title">Непорожня назва товару.</param>
    /// <param name="price">Додатна облікова ціна.</param>
    /// <param name="stock">Початкова кількість одиниць.</param>
    public Product(string sku, string title, decimal price, int stock)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(sku);
        ArgumentException.ThrowIfNullOrWhiteSpace(title);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(price);
        ArgumentOutOfRangeException.ThrowIfNegative(stock);
        Sku = sku;
        Title = title;
        Price = price;
        Stock = stock;
    }

    /// <summary>Артикул товару.</summary>
    public string Sku { get; }

    /// <summary>Назва товару.</summary>
    public string Title { get; }

    /// <summary>Облікова ціна одиниці.</summary>
    public decimal Price { get; }

    /// <summary>Залишок, що ніколи не стає від’ємним.</summary>
    public int Stock { get; private set; }

    /// <summary>Збільшує залишок на додатну кількість.</summary>
    /// <param name="quantity">Кількість прийнятих одиниць.</param>
    public void Receive(int quantity)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(quantity);
        Stock = checked(Stock + quantity);
    }

    /// <summary>Видає доступну додатну кількість товару.</summary>
    /// <param name="quantity">Кількість відвантажених одиниць.</param>
    public void Dispatch(int quantity)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(quantity);
        if (quantity > Stock)
        {
            throw new InvalidOperationException("Недостатній залишок товару.");
        }

        Stock -= quantity;
    }
}
