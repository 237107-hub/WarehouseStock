namespace WarehouseStock.Core.Domain;

/// <summary>Незмінний запис довідника складів як учасників переміщення.</summary>
public sealed record Warehouse
{
    /// <summary>Створює склад із перевіреними реквізитами.</summary>
    /// <param name="id">Додатний ідентифікатор складу.</param>
    /// <param name="name">Непорожня назва складу.</param>
    /// <param name="email">Контактна пошта з символом @.</param>
    public Warehouse(int id, string name, string email)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(id);
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentException.ThrowIfNullOrWhiteSpace(email);
        if (!email.Contains('@'))
        {
            throw new ArgumentException("Пошта має містити @.", nameof(email));
        }

        Id = id;
        Name = name;
        Email = email;
    }

    /// <summary>Ідентифікатор складу.</summary>
    public int Id { get; }

    /// <summary>Назва складу.</summary>
    public string Name { get; }

    /// <summary>Контактна адреса пошти.</summary>
    public string Email { get; }
}
