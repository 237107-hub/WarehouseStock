# WarehouseStock — лабораторна робота 4

Конструювання класів: інкапсуляція та SOLID.
Здравков Максим Юрійович, група 471, варіант 6.

## Запуск

Розпакуйте весь ZIP. Для готової демонстрації відкрийте Запустить.cmd у корені
архіву (потрібне середовище .NET 8 для Windows x64). Для редагування відкрийте
WarehouseStock.sln у Visual Studio; стартовий проєкт — WarehouseStock.App.
Бібліотеку WarehouseStock.Core запускати безпосередньо не можна.

```powershell
dotnet restore
dotnet build -c Release
dotnet test -c Release
dotnet run --project WarehouseStock.App -c Release
dotnet run --project WarehouseStock.App -c Release -- vip
```

Потрібен .NET SDK 8+ з підтримкою net8.0 і NuGet при першому restore.
Перевірено SDK 9.0.318. xUnit 2.9.2, Microsoft.NET.Test.Sdk 17.12.0.
Без аргументу обирається regular: 2 × 250 зі знижкою 5% = 475.00.
Режими vip / staff / standard дають 450.00 / 350.00 / 500.00.
Невідомий ключ політики обирає standard. Усі залежності складає Program.cs.

## Модель

- Domain/TransferLine та Warehouse — sealed record, get-only, перевірки в ctor.
- Domain/Transfer — приватний List, захищене ReadOnlyCollection-подання,
  private set стану, AddLine / Approve / Dispatch / Complete / Cancel.
- Domain/Product — get-only реквізити, Stock змінюється лише Receive / Dispatch.
- Domain/ArchivedTransfer — композиція закритого Transfer без AddLine.
- Abstractions — ITransferRepository (3 методи), IPricingPolicy (1), INotifier (1).
- Storage — одна реалізація сховища в пам’яті на Dictionary.
- Pricing — 3 взаємозамінні політики; Notifications — локальний TextWriter.
- Reporting/TransferCsvReport — чисте формування CSV зі знімків значень.

Документ не виконує автоматичне списання запасів двох складів: транзакції
залишків поза обсягом роботи. Пошта перевіряється правилом @, SMTP немає.
Архів дозволений тільки у Completed/Cancelled; інваріант стабільності не
залежить від добросовісності зовнішнього власника посилання на документ.
Новий модуль ЛР4 не заявляє числової сумісності з дефектним класом-Богом:
його безіменне віднімання 50 не є частиною контракту нових політик.

## Перевірки та історія

```powershell
dotnet format --verify-no-changes
dotnet build -c Release -t:Rebuild
dotnet test -c Release --filter FullyQualifiedName~DomainInvariantTests
dotnet test -c Release
git --no-pager log --oneline lab3..HEAD
```

Рівно 3 нові тести ЛР4 у DomainInvariantTests; 38 регресійних тестів ЛР3
збережено, у повному наборі 41. Додатково виконано 25 інтеграційних перевірок
без файлових операцій та SMTP. Збірка — 0 помилок і попереджень.
Еталон класу-Бога розміщено тільки в tests/WarehouseStock.Tests/Legacy;
Core не містить Console чи небезпечних успадкованих реалізацій.

docs/model.md — початкова модель; docs/lab04 — аудит із рівно 5 рядками,
порівняння, журнали і XML-результати перевірок. Історичні docs/lab02 та lab03
залишені для попередніх станів. Гілка lab04-classes-solid; теги lab1–lab4.
Віддалений origin не налаштовано, push не виконувався. .git входить до архіву;
для здачі посилання потрібно опублікувати репозиторій у власному GitHub/GitLab.
