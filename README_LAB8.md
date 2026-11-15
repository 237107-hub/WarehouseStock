# Лабораторна робота 8 WarehouseStock

Здравков Максим Юрійович, група 471, варіант 6. Код опубліковано, PR #1 об’єднано, Actions main зелений.

## Відтворення
1. Розпакуйте весь ZIP. Запуск.cmd відкриває програму з версією 0.8.0; потрібен .NET 8 Runtime.
2. Для розробки встановіть SDK 9.0.318 (global.json) і runtime .NET 8, відкрийте WarehouseStock.sln.
3. eng/quality.ps1 робить locked restore, format verify, Release build, тести й поріг покриття 80%. Потрібен доступ до NuGet.
4. dotnet run --project WarehouseStock.App -- --lab8 показує приклади прийняття та відхилення вводу.
5. Coverage/index.html у ZIP показує справжнє локальне покриття; Verification містить протоколи.

Результат: 146 тестів; 90.14% рядків і 88.46% гілок Core; Release 0 попереджень і помилок. Негативні локальні перевірки CS0168 та порога 99% провалилися, після виправлення всі критерії пройдено. Такі самі результати підтверджено реальними GitHub Actions нижче.

Секретів за переглянутими шаблонами не виявлено; це не математичний доказ їх відсутності. Аудит NuGet перевірив прямі та транзитивні пакети, відомих уразливих пакетів не виявив. Через недоступний системний HTTP-кеш/HTTPS у цьому середовищі команди виконано з локальною копією офіційних метаданих NuGet, отриманою через HTTPS; джерела і SHA256 записано в Verification. Репозиторій використовує звичайний HTTPS NuGet.Config, локального HTTP-сервера в ньому немає.

## Фактична публікація


Репозиторій: https://github.com/237107-hub/WarehouseStock
PR: https://github.com/237107-hub/WarehouseStock/pull/1 — squash and merge у main, коміт 12f6d05b851bb77fcb6bc110f3645120f48af93b.
Гілку feature/ci-pipeline видалено після злиття; історія доступна у PR.

- Червоний Actions CS0168: https://github.com/237107-hub/WarehouseStock/actions/runs/43712245001
- Зелений Actions гілки: https://github.com/237107-hub/WarehouseStock/actions/runs/43713370650
- Зелений Actions PR: https://github.com/237107-hub/WarehouseStock/actions/runs/43714662340
- Зелений Actions main після злиття: https://github.com/237107-hub/WarehouseStock/actions/runs/43714954711

У хмарі пройшли 146 тестів, Core 90.14% рядків і 88.46% гілок. TRX і Cobertura збережено в quality-evidence. Реальні скриншоти і JSON протоколи додаються до звіту та архіву.
ЛР5–7 і теги lab1–lab7 опубліковані. Тег v0.8.0 публікує finish_github.ps1 після надсилання цієї документації та зеленого Actions на її коміті.
