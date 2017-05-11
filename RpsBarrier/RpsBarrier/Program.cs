using System;
using System.Diagnostics;
using System.Threading;

namespace RpsBarrier
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.BufferHeight = 10000;

            var forInit = RpsBarrier.Instance;
            RunTest("#1", 5, 50, 10, 300);
            RunTest("#2", 50, 1000, 10, 30);
            RunTest("#3", 2000, 20000, 0, 0);

            Console.WriteLine();
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

            //RpsBarrier.Instance.Reset();
            RpsBarrier.Instance.MaxOperationsPerSecond = rps;
            var rnd = new Random(DateTime.UtcNow.Millisecond);
            var sw = new Stopwatch();

            for (var i = 0; i < iterationsCount; i++)
            {
                sw.Start();
                var res = RpsBarrier.Instance.CanExecute();
                sw.Stop();
                var dt = DateTime.UtcNow;
                Console.WriteLine($"{i + 1, 6}. {dt.ToString("T"),8}.{dt.Millisecond, -3} - {res, -5} | Elapsed {sw.ElapsedTicks} ticks");
                sw.Reset();
                if (minRnd <= maxRnd && maxRnd > 0)
                    Thread.Sleep(rnd.Next(minRnd, maxRnd + 1));
            }
            
            Console.WriteLine("Test completed. Press any key to continue...");
            Console.ReadKey();
        }

    }
}
