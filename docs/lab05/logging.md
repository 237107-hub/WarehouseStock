# Журналювання та контекст операції

Microsoft.Extensions.Logging.Abstractions 8.0.2 у Core;
Microsoft.Extensions.Logging.Console 8.0.1 у App. Офіційні пакети завантажено
з NuGet. ILogger<TransferService> — четверта залежність конструктора.

Увімкнено Debug; SimpleConsole додає час та IncludeScopes. TransferLog.Scope
використовує кешовану LoggerMessage.DefineScope<int>("Transfer:{TransferId}").
Події описані атрибутами LoggerMessage зі сталими шаблонами й EventId;
згенеровані методи зберігають поля TransferId, Amount, AllowedValue, Extra,
Sku, Quantity, Reason окремо від тексту. Це узгоджено з активним CA1848;
правило не вимикалося. Виняток передається окремим параметром, не ex.Message.

Демонстрація містить Debug, Information, Warning та Error; IOException
штучно створено DemoTransferRepository, це явно повідомлено користувачеві.
Одноразовий повтор робить зовнішній TransferScenario, а не ядро. Реального
файлового сховища чи мережевого запиту не імітуємо як справжні.
Critical лишено для непередбаченої помилки у верхній межі Program.cs.

Result<T> — readonly struct. Його фабрики винесено у неузагальнений
ResultFactory.Ok/Fail, щоб не вимикати CA1000. Value невдалого результату
захищено винятком; default-результат має осмислену помилку, а не порожній успіх.

Структурованість, 4 рівні, контекст Transfer:1, первинний IOException,
повторний throw та закриття scope підтверджені автоматичними тестами.
