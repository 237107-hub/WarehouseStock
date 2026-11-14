# Чотири антипатерни та їх виправлення

Навчальні до-версії — текстові лістинги .cs.txt, не частина продуктивної збірки.
Вони додані окремим комітом antipatterns: before: це дозволяє зберегти сам
дефект і водночас не вимикати CA2200/інші аналізатори для робочого рішення.
Виправлення Task5 та логування Task6 природно зачіпають той самий Load,
тому зафіксовані разом у наступному коміті зі строгим складанням.

| Антипатерн | До: файл і рядок | Чим замінено: файл і рядок | Усунутий наслідок |
|---|---|---|---|
| Проковтнутий catch | docs/lab05/before/ExceptionAntipatterns.cs.txt:8 | WarehouseStock.Core/Services/TransferService.cs:74; відсутність — transfer.exists, IOException — журнал і throw | Помилка не перетворюється непомітно на null |
| Надто широкий catch | docs/lab05/before/ExceptionAntipatterns.cs.txt:21 | WarehouseStock.Core/Services/TransferService.cs:83; catch (Exception) лише в Program.cs | Помилки програміста не маскуються як збій сховища |
| Втрата стека throw exception | docs/lab05/before/ExceptionAntipatterns.cs.txt:24 | WarehouseStock.Core/Services/TransferService.cs:86; повторне кидання того самого винятку | Збережені початкова точка й стек GetById |
| Винятки як звичайне керування | docs/lab05/before/ExceptionAntipatterns.cs.txt:35 | WarehouseStock.App/Input/QuantityCounter.cs:17; TryParse та умова quantity > 0 | Нечислові та завеликі рядки — звичайна невдача перевірки |

У Core не лишилося catch (Exception), throw exception, int.Parse, decimal.Parse
чи Console. Історичні еталони ЛР3/ЛР4 в тестах не є ядром нового застосунку.
Прямий nullable-пошук сховища лишився нормальним optional-контрактом;
обов’язковий запит сервісу Load перетворює його відсутність на іменоване правило.
