using System;
using System.Threading;

namespace RpsBarrier
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.BufferHeight = 10000;

            RunTest("#1", 5, 50, 10, 300);
            RunTest("#2", 50, 1000, 10, 30);

            Console.CursorTop += 1;
            Console.WriteLine("All tests completed. Press any key to exit...");
            Console.ReadKey();
        }

        /// <summary>
        /// Запустить тест
        /// </summary>
        /// <param name="testName">Название теста</param>
        /// <param name="rps">Максимальное количество операций за секунду</param>
        /// <param name="iterationsCount">Количество итераций цикла</param>
        /// <param name="minRnd">Минимальное число милисекунд сна</param>
        /// <param name="maxRnd">Максимальное число милисекунд сна</param>
        static void RunTest(string testName, int rps, int iterationsCount, int minRnd = 0, int maxRnd = 0)
        {
            Console.WriteLine($"Test {testName}. Iterations count: {iterationsCount}. RPS: {rps}");
            Console.WriteLine("Press any key to start...");
            Console.ReadKey();
            Console.CursorTop -= 1;
            Console.WriteLine(new string(' ', 79));
            Console.CursorTop -= 1;

            RpsBarrier.Instance.MaxOperationsPerSecond = rps;
            var rnd = new Random(DateTime.UtcNow.Millisecond);

            for (var i = 0; i < iterationsCount; i++)
            {
                var res = RpsBarrier.Instance.CanExecute();
                var dt = DateTime.UtcNow;
                Console.WriteLine($"{i,5}. {dt.ToString("T"),8} {dt.Millisecond,3} - {res}");
                if (minRnd <= maxRnd && maxRnd > 0)
                    Thread.Sleep(rnd.Next(minRnd, maxRnd));
            }

            Console.WriteLine("Test completed. Press any key to continue...");
            Console.ReadKey();
        }

    }
}
