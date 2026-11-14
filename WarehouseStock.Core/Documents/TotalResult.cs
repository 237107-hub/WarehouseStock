namespace WarehouseStock.Core.Documents;

/// <summary>Результат без зміни вхідного документа.</summary>
/// <param name="Total">Підсумкова сума.</param>
/// <param name="NewStatus">Наступний стан документа.</param>
public sealed record TotalResult(decimal Total, DocumentStatus NewStatus);
