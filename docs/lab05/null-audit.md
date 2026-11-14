# Аудит повернення null

| Метод | Було / призначення | Рішення після ЛР5 |
|---|---|---|
| ITransferRepository.GetById та InMemoryTransferRepository.GetById | Optional-пошук Transfer? | Залишено: відсутність ключа є нормальною відповіддю запиту сховища. Nullable позначено в контракті. |
| TransferService.Load | Новий обов’язковий запит замість навчального Load із catch/null | Повертає Transfer; відсутність перетворює на DomainRuleException transfer.exists. |
| Legacy.ILegacyTransferStore.GetById у тестовому еталоні ЛР4 | Не реалізований широкий історичний контракт | Не використовується продуктом; залишений тільки як доказ аудиту попередньої роботи. |

Інших методів продуктового Core з null як ознакою «не знайдено» немає.
Result<T>.Value не є optional-пошуком: доступ до значення невдалого результату
кидає InvalidOperationException. Успішне значення не може бути null.

Approve повертав навчальні -1/-2/-3 у docs/lab05/before/RawTransferService.cs.txt.
Новий метод повертає void і кидає іменоване порушення правила з конкретними
значеннями: transfer.exists, transfer.approvable, transfer.lines, transfer.amount.
Інваріанти аргументів захищають ArgumentNull/OutOfRangeException; вони не є
помилками рядкового вводу. Клієнт верхнього шару окремо обробляє DomainRuleException.
У робочому Core кодів повернення -1/-2/-3 немає. Навчальні до-версії та еталони
ЛР3/ЛР4 у тестах не використовуються як API продукту.
