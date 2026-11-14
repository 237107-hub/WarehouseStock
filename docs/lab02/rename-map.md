# Карта перейменувань

Переіменування виконано через Roslyn RenameSymbolAsync із перевіркою символів C#. Рядкові літерали та коментарі не замінювалися.

| Було | Стало | Обґрунтування |
|---|---|---|
| peremishchennia | StockTransfer | Тип англійською, PascalCase |
| id | Id | Ідентифікатор документа |
| cl | RecipientName | Назва одержувача |
| ln | _lines | Приватне поле _camelCase |
| st | Status | Зміст стану |
| dt | CreatedAt | Час створення документа |
| Add | AddLine | Дія та результат методу |
| ProcessData | CalculateTotal | Дія та результат методу |
| Chg | TryChangeStatus | Дія та результат методу |
| Ck | IsValid | Дія та результат методу |
| Rep | BuildReport | Дія та результат методу |
| F | FindById | Дія та результат методу |
| .ctor: a | id | Параметр camelCase за його роллю |
| .ctor: b | recipientName | Параметр camelCase за його роллю |
| AddLine: s | sku | Параметр camelCase за його роллю |
| AddLine: k | quantity | Параметр camelCase за його роллю |
| AddLine: c | unitPrice | Параметр camelCase за його роллю |
| CalculateTotal: f | isRegularRecipient | Параметр camelCase за його роллю |
| TryChangeStatus: n | newStatus | Параметр camelCase за його роллю |
| FindById: L | transfers | Параметр camelCase за його роллю |
| FindById: x | transferId | Параметр camelCase за його роллю |
| AddLine: t | line | Ім’я за фактичною дією коду |
| CalculateTotal: sum1 | total | Ім’я за фактичною дією коду |
| CalculateTotal: kolvo | lineCount | Ім’я за фактичною дією коду |
| CalculateTotal: k | quantity | Ім’я за фактичною дією коду |
| CalculateTotal: c | unitPrice | Ім’я за фактичною дією коду |
| BuildReport: s | report | Ім’я за фактичною дією коду |
| Program.cs: z | transfer | Документ переміщення |

Поля Id, RecipientName, Status і CreatedAt замінено властивостями з private set. Список _lines є приватним readonly-полем. Числа та порядок розрахунків залишено без змін.
