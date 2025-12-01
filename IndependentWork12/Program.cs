using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace IndependentWork12
{
    class Program
    {
        // Налаштування експерименту
        static readonly int[] Sizes = new int[] { 1_000_000, 5_000_000 /*, 10_000_000 */ }; 
        // Примітка: 10M може вимагати багато пам'яті; розкоментуй, якщо система витримає.
        static readonly int Repeats = 3; // кількість повторів для усереднення часу

        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.WriteLine("IndependentWork12 — PLINQ: дослідження продуктивності та безпеки");
            Console.WriteLine("---------------------------------------------------------------");
            Console.WriteLine();

            // Для кожного розміру даних виконаємо серію вимірювань
            foreach (var size in Sizes)
            {
                Console.WriteLine($"--- Розмір колекції: {size:N0} елементів ---");
                // Генеруємо дані (цілочисельні випадкові числа)
                var data = GenerateRandomList(size, seed: 12345); // фіксований seed для повторюваності

                // JIT / warm-up: викличемо один раз IsPrime, щоб уникнути впливу JIT на вимірювання
                IsPrime(7919);

                // 1) Вимірювання для операції: перевірка на просте число (обчислювальна операція)
                MeasureLinqVsPlinq(
                    data,
                    x => IsPrime(x),
                    $"Перевірка на просте число (IsPrime) — size={size:N0}",
                    repeats: Repeats
                );

                Console.WriteLine();

                // 2) Вимірювання для важчої операції — "псевдо-факторіал": витратна числова трансформація
                MeasureLinqVsPlinq(
                    data,
                    x => HeavyMath(x),
                    $"Важка математична операція HeavyMath (итерована трансформація) — size={size:N0}",
                    repeats: Repeats,
                    isPredicate: false
                );

                Console.WriteLine(new string('-', 70));
                Console.WriteLine();
            }

            // Дослідження проблем з побічними ефектами (thread-safety)
            DemonstrateSideEffects();

            Console.WriteLine("\nГотово. Натисніть будь-яку клавішу для виходу.");
            Console.ReadKey();
        }

        // Генерація даних
        static List<int> GenerateRandomList(int size, int min = 2, int max = 1_000_000, int seed = 0)
        {
            var rnd = seed == 0 ? new Random() : new Random(seed);
            var list = new List<int>(size);
            for (int i = 0; i < size; i++)
            {
                // заповнюємо випадковими числами в діапазоні [min, max)
                list.Add(rnd.Next(min, max));
            }
            return list;
        }

        // Вимірювання LINQ vs PLINQ
        // Якщо isPredicate == true => використовуємо Where + Count/ToList
        // Якщо isPredicate == false => використовуємо Select (трансформацію) + ToList
        static void MeasureLinqVsPlinq<T>(List<int> data, Func<int, T> operation, string title, int repeats = 3, bool isPredicate = true)
        {
            Console.WriteLine($"Експеримент: {title}");
            // Запустимо кілька повторів і виведемо середній час
            double totalLinq = 0, totalPlinq = 0;
            for (int r = 1; r <= repeats; r++)
            {
                Console.WriteLine($"\nПовтор #{r}:");

                // LINQ (послідовна)
                var sw = Stopwatch.StartNew();
                if (isPredicate)
                {
                    var result = data.Where(x => (bool)(object)operation(x)).ToList(); // небезпека касту контролюється викликом
                    // для предиката ми зберігаємо вибрані елементи у result
                    // (використовуємо ToList(), щоб примусити повну обробку)
                }
                else
                {
                    var result = data.Select(x => operation(x)).ToList(); // трансформація
                }
                sw.Stop();
                Console.WriteLine($"LINQ (послідовний) час: {sw.Elapsed.TotalSeconds:F3} с");
                totalLinq += sw.Elapsed.TotalSeconds;

                // PLINQ (паралельний)
                sw.Restart();
                if (isPredicate)
                {
                    var result = data.AsParallel().Where(x => (bool)(object)operation(x)).ToList();
                }
                else
                {
                    var result = data.AsParallel().Select(x => operation(x)).ToList();
                }
                sw.Stop();
                Console.WriteLine($"PLINQ (паралельний) час: {sw.Elapsed.TotalSeconds:F3} с");
                totalPlinq += sw.Elapsed.TotalSeconds;
            }

            Console.WriteLine($"\nСередній час (LINQ)  = {totalLinq / repeats:F3} с");
            Console.WriteLine($"Середній час (PLINQ) = {totalPlinq / repeats:F3} с");

            if ((totalPlinq / repeats) < (totalLinq / repeats))
                Console.WriteLine("Висновок: PLINQ швидший для цієї операції в даних умовах.");
            else
                Console.WriteLine("Висновок: LINQ/послідовна версія виявилася швидшою або порівнянною (ймовірні накладні витрати на паралелізацію).");
        }

        // Обчислювально інтенсивна операція: перевірка на просте число (IsPrime)
        // Простий, але достатньо витратний алгоритм (trial division)
        static bool IsPrime(int n)
        {
            if (n <= 1) return false;
            if (n <= 3) return true;
            if (n % 2 == 0) return n == 2;

            int r = (int)Math.Sqrt(n);
            for (int i = 3; i <= r; i += 2)
            {
                if (n % i == 0) return false;
            }
            return true;
        }

        // Інша важка операція — "HeavyMath"
        // Повертає подвійне значення; виконує серію чисельних операцій, щоб створити навантаження CPU
        // Використовується для демонстрації трансформацій (Select)
        static double HeavyMath(int x)
        {
            // Серія математичних операцій, які дають відчутне навантаження
            double v = x;
            v = Math.Log(v + 1) * Math.Sqrt(v + 1);
            // ітераційні операції для збільшення часу
            for (int i = 0; i < 10; i++)
            {
                v = Math.Pow(v + i, 1.0001 + (i % 3) * 0.01);
                v = Math.Sin(v) + Math.Cos(v);
            }
            return v;
        }

        // Демонстрація проблем з побічними ефектами
        // 1) buggy: PLINQ змінює спільну змінну без синхронізації -> race condition
        // 2) fixed: використано Interlocked або Concurrent collection для безпеки
        static void DemonstrateSideEffects()
        {
            Console.WriteLine("=== Демонстрація побічних ефектів та потокобезпечних рішень ===");
            int size = 1_000_000;
            var data = GenerateRandomList(size, seed: 54321);

            Console.WriteLine("\n1) Небезпечний приклад: інкремент sharedCounter у PLINQ без синхронізації");
            int sharedCounter = 0;

            // Виконуємо паралельну обробку і інкрементуємо в залежності від IsPrime (rac condition)
            data.AsParallel().ForAll(x =>
            {
                if (IsPrime(x))
                {
                    // Небезпечно: одночасні записи в sharedCounter
                    sharedCounter++;
                }
            });

            Console.WriteLine($"Очікуваний (правильний) підрахунок простих чисел (послідовний): {data.Count(IsPrime)}");
            Console.WriteLine($"Отриманий (ненадійний) підрахунок у PLINQ без синхронізації: {sharedCounter}");

            Console.WriteLine("\n2) Виправлення: використання Interlocked.Increment для атомарного інкременту");
            int atomicCounter = 0;
            data.AsParallel().ForAll(x =>
            {
                if (IsPrime(x))
                {
                    Interlocked.Increment(ref atomicCounter);
                }
            });
            Console.WriteLine($"Підрахунок з Interlocked.Increment: {atomicCounter}");

            Console.WriteLine("\n3) Інший варіант: використання потокобезпечної структури (ConcurrentBag) і Count");
            var bag = new ConcurrentBag<int>();
            data.AsParallel().ForAll(x =>
            {
                if (IsPrime(x))
                {
                    bag.Add(x);
                }
            });
            Console.WriteLine($"Підрахунок з ConcurrentBag: {bag.Count}");
        }
    }
}
