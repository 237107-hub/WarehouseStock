namespace WarehouseStock.Core.Documents;

/// <summary>Відомості про одержувача переміщення.</summary>
/// <param name="Name">Назва складу або одержувача.</param>
/// <param name="Email">Адреса пошти.</param>
/// <param name="IsRegular">Ознака постійного одержувача.</param>
public sealed record RecipientInfo(string Name, string Email, bool IsRegular);
