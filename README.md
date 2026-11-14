# WarehouseStock

Лабораторна робота 3 — декомпозиція підпрограм та захисне програмування.
Здравков Максим Юрійович, група 471, варіант 6.
Навчальний розрахунок облікової вартості переміщення товарів між складами.

## Відкриття

Розпакувати ВЕСЬ архів до звичайної папки. Відкрити WarehouseStock.sln,
а не файл усередині ZIP. У Visual Studio стартовим призначити WarehouseStock.App.
Потрібні .NET SDK 8+ із середовищем .NET 8 та доступ до NuGet при першому restore.
Перевірено SDK 9.0.318. xUnit 2.9.2; Microsoft.NET.Test.Sdk 17.12.0.

```powershell
dotnet restore
dotnet build -c Release
dotnet test -c Release
dotnet run --project WarehouseStock.App -c Release
```

Демонстрація: 2 × 200 = 400; SALE10 → 360; доставка 60 → 420.00;
Approved → Dispatched. Програма друкує документ українською.
Для власного документа змініть init-властивості request у Program.cs.
Calculate не друкує; TextOf формує текст; Print записує до TextWriter.
Для збереження звіту замість Console.Out можна передати StreamWriter.

## Склад

- Core/Documents — guards, enum, DTO, 14 констант, 7 правил і Calculate.
- Core/Reporting — чиста побудова тексту й окрема команда друку.
- Core/Domain — базові сутності попередніх лабораторних.
- tests/WarehouseStock.Tests — 38 тестів, зокрема 200 порівнянь з еталоном.
- docs/lab03 — фактичні метрики, обґрунтування й журнали виконання.
- docs/lab02 — історичні матеріали другої роботи.

## Перевірки

```powershell
dotnet format --verify-no-changes
dotnet build -c Release -t:Rebuild
dotnet test -c Release --no-build
git --no-pager log --oneline lab2..HEAD
```

Підсумок: 0 помилок і попереджень; 38/38 тестів. Максимум ЦС 4,
3 параметри явно написаного методу, 17 фізичних рядків тіла.
У Core немає Console. Контракти відхиляють некоректні аргументи винятками.
Незмінні DTO не означають глибокої незмінності IReadOnlyList; не змінюйте
список паралельно з обчисленням. Недільну дату передано явно.

Гілка lab03-subprograms. Теги lab1 та lab2 збережені; lab3 позначає фінал.
LegacyReference існує тільки в тестах як зафіксований еталон для порівняння;
його довгий метод не входить до метрик продуктивної бібліотеки.
Віддалений origin не налаштований. Архів містить .git з усією історією;
для здачі посилання завантажте репозиторій у власний GitHub/GitLab.
Скріншоти термінала не створювалися: докази — оригінальні текстові журнали
та результати запуску xUnit у docs/lab03/evidence.
