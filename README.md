# WarehouseStock — лабораторна робота 5

Обробка помилок, винятків і логування.
Здравков Максим Юрійович, група 471, варіант 6.

## Початок

Розпакуйте весь архів. Запустить.cmd показує сценарій: кількість «дві»
відхиляється, потім «2» приймається; одноразовий штучний IOException
залоговано, погодження повторено, сума 180.00. Це позначена демонстрація,
не реальний збій диска. Для власного вводу відкрийте Ввод.cmd.
У ціні потрібна крапка (10.50); тисячні роздільники й експоненти не приймаються.
Готовий Run потребує .NET 8 для Windows x64; для вихідного рішення потрібен SDK 8+.

## Збирання й перевірка

Відкрити WarehouseStock.sln у Visual Studio; стартовий WarehouseStock.App.
Для першого restore потрібен NuGet або готовий локальний кеш.

```powershell
dotnet restore
dotnet build -c Release
dotnet test -c Release
dotnet run --project WarehouseStock.App -c Release
dotnet run --project WarehouseStock.App -c Release -- --interactive
dotnet format --verify-no-changes
```

У default-сценарії введення повторюване; --interactive використовує Console.In
та не вмикає штучний збій сховища. Код виходу: 0 успіх; 1 правило або кінець
вводу; 2 непередбачений дефект; 3 IOException; 4 відмова доступу.
Ліміт погодження у демонстрації — 200; знижка — 10%.

## Реалізація

- App/Input/TransferLineParser: рядки → Result<Domain.TransferLine>.
- Core/Errors/Result: readonly struct; ResultFactory без вимкнення CA1000.
- DomainRuleException: Rule, контекстне повідомлення й innerException.
- TransferService.Load/AddLine/Approve/Cancel: типізовані операції й правила.
- Core/Diagnostics/TransferLog: LoggerMessage-події та DefineScope з TransferId.
- Program.cs: єдиний загальний catch(Exception), конкретні межі й LoggerFactory.
- FileAuditLog та TransferReportExporter: using, IDisposable; після винятку
  файл можна ексклюзивно відкрити знову. Дані — Run/data для готової програми.
- docs/errors.md: 9 випадків, по 3 на кожний клас; docs/lab05 — аудит і висновки.

Збірка Release: 0 попереджень; 65/65 тестів, з них 24 нових у ЛР5.
Збережено 41 регресійний тест. Контракт сервісу навмисно доповнено четвертим
параметром ILogger; попередній тест DI оновлено з NullLogger.
Пошук репозиторію GetById залишається nullable за optional-контрактом;
обов’язковий Load більше не поширює null. Великі бізнес-суми окремо не обмежені:
переповнення decimal на верхній межі дає Critical/код 2, а не невірний підсумок.

Операційні .log, data, bin, obj до Git не додаються. Навчальні «до» лежать
у docs/lab05/before як .cs.txt, аби не вимикати аналізатори заради поганих
фрагментів. Попередній стан і виправлення видно в різних комітах.
Гілка lab05-error-handling; тег lab5. Архів містить .git з повною історією.
Віддалений origin не налаштовано й push не виконувався; для здачі посилання
треба опублікувати репозиторій у власному GitHub/GitLab.
