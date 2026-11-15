# Реєстр запахів до рефакторингу

| Запах | Місце та ознака | Рефакторинг | Пріоритет |
|---|---|---|---|
| Duplicate Code | Handle 70–93 і Preview 115–139: сума, три ставки, доставка | Extract Method, спільні політики | високий |
| Long Method | Handle 31–110: перевірки, ціни, звіт і журнал | Extract Method | високий |
| Large Class | LegacyTransferProcessor: 7 обов'язків | Extract Class | високий |
| Long Parameter List | Handle: 11 параметрів, два не використовуються | Introduce Parameter Object | високий |
| Primitive Obsession | рядки vip/staff/new/paid | Replace Type Code with Enum | середній |
| Feature Envy | DescribeClient читає 5 полів іншого об'єкта | Move Method | середній |
| Nested Conditionals | Handle: 4 вкладені if | Replace Nested Conditional with Guard Clauses | високий |
| Temporary Field | _tmpSum, _tmpDiscount: потрібні одному виклику | Replace Temp with local variable | високий |
| Magic Numbers | 0.15, 0.30, 0.05, 500, 1000, 60, 10 | Replace Magic Number with Constant | середній |
| Dead Parameters | clientId, createdAt не впливають на результат | Remove Parameter у новому запиті | низький |

Точні номери рядків і метрики зберігаються у metrics-before.json. Початкові винятки аналізаторів обмежено src/Legacy: CA1051, CA1847, CA1305, CA1304, CA1311, CA1822. Це дозволяє спочатку зафіксувати незмінений алгоритм методички, а потім поступово прибрати винятки.
