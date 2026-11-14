# Аудит навчального класу-Бога

Еталон — tests/WarehouseStock.Tests/Legacy/*.cs. Відтворено інженерні дефекти
методички; форматування узгоджено, для чисел явно вказано InvariantCulture.
SmtpMailer є локальною імітацією без SMTP. FileTransferStore потрібен для
компіляції еталона; перевірки його не викликають. Порушення не перенесено до Core.

| Принцип | Доказ у коді | Конкретна майбутня зміна та наслідок | Засіб |
|---|---|---|---|
| SRP | TransferManager.Place рахує, змінює стан, зберігає і сповіщає; BuildCsvReport ще й пише файл | Зміна теми повідомлення або CSV-роздільника змушує редагувати клас сценарію | TransferService, InMemoryTransferRepository, TextWriterNotifier, TransferCsvReport |
| OCP | Place: if recipientType == regular / vip / staff | Для нового типу одержувача треба додати гілку в наявний метод | Реалізації IPricingPolicy; вибір у Program.cs |
| LSP | Archive: LegacyTransfer archived = new ArchivedLegacyTransfer(...); archived.AddLine(line) | Для непорожнього документа Archive впаде з NotSupportedException замість додавання позиції | ArchivedTransfer містить закритий Transfer, не успадковує його і не має AddLine |
| ISP | ILegacyTransferStore має 7 методів, включно з SendEmail, Backup, PrintInvoice | Нова вимога до друку змінює інтерфейс сховища, хоча сценарію потрібен лише SaveToFile | ITransferRepository з 3 методами та INotifier з 1 методом |
| DIP | Поля _store = new FileTransferStore(), _mailer = new SmtpMailer() | Заміна файла базою даних потребує зміни бізнес-класу; важко підставити тестові залежності | readonly-поля абстракцій; усі залежності передаються конструктору |

## Два числові висновки

TransferManager має 7 визначених незалежних причин для зміни:
1. Правила оцінювання/знижок.
2. Правила переходу стану документа.
3. Формат та адреса зберігання документа.
4. Канал, адресат і шаблон повідомлення.
5. Склад полів та формат CSV.
6. Куди записувати CSV і як підтверджувати його виведення.
7. Спосіб формування й зберігання архівного подання.

З 7 методів ILegacyTransferStore цьому менеджеру фактично потрібен 1:
SaveToFile. Він викликається у Place та Archive; два місця виклику не є двома
різними методами. Сам менеджер навіть не залежить від цього інтерфейсу —
ще один прояв DIP. У новій моделі сервіс користується Add та Notify;
GetById і GetAll потрібні клієнтам сховища, а не сценарію реєстрації.

## Виправлення

| Принцип | Було | Стало |
|---|---|---|
| SRP | TransferManager.Place / BuildCsvReport | Сервіс сценарію, сховище, сповіщувач, чистий CSV-форматувальник |
| OCP | if/else за recipientType | RegularPricingPolicy, DiscountPricingPolicy, VipPricingPolicy |
| LSP | ArchivedLegacyTransfer : LegacyTransfer | ArchivedTransfer із приватним Transfer усередині |
| ISP | Один ILegacyTransferStore на 7 операцій | ITransferRepository та INotifier з окремими контрактами |
| DIP | new залежностей у полях менеджера | Конструктор TransferService з трьома абстракціями; збірка в Program.cs |

ЛР4 демонструє заміну організації коду, а не гарантує числову еквівалентність
дефектному менеджеру: його додаткову безіменну знижку 50 понад 1000 не включено
до контракту нових політик. Політики і правила ЛР4 явно задані в docs/model.md.
Послідовний розрахунок ЛР3 та його 38 тестів не змінено.
