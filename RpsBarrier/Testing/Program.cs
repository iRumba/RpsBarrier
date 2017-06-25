using RpsBarrier;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Testing
{
    class Program
    {
        static object _lockObj = new object();
        static int _number;

        static void Main(string[] args)
        {
            RunTest("One Thread", 50, 280 ,5, 15);
            RunTestMultithread("Multi Thread", 50, 280, 4, 20, 60);
            Console.WriteLine("All tests were completed. Press any key to exit...");
            Console.ReadKey();
        }

        static void RunTest(string testName, int rps, int iterationsCount, int minRnd = 0, int maxRnd = 0)
        {
            Console.WriteLine($"Test {testName}. Iterations count: {iterationsCount}. RPS: {rps}");
            Console.WriteLine("Press any key to start...");
            Console.ReadKey();
            Console.CursorTop -= 1;
            Console.WriteLine(new string(' ', 79));
            Console.CursorTop -= 1;

            var rpsPool = new RpsPool(rps);
            var rnd = new Random(DateTime.UtcNow.Millisecond);
            var sw = new Stopwatch();

            for (var i = 0; i < iterationsCount; i++)
            {
                sw.Start();
                var res = rpsPool.CanExecute();
                sw.Stop();
                var dt = DateTime.UtcNow;
                Console.WriteLine($"{i + 1,6}. {dt.ToString("T"),8}.{dt.Millisecond,-3} - {res,-5} | Elapsed {sw.ElapsedTicks} ticks");
                
                if (minRnd <= maxRnd && maxRnd > 0)
                    Thread.Sleep(rnd.Next(minRnd, maxRnd + 1));
                sw.Reset();
            }

            Console.WriteLine("Test completed. Press any key to continue...");
            Console.ReadKey();
        }

        static void RunTestMultithread(string testName, int rps, int iterationsCount, int threads, int minRnd = 0, int maxRnd = 0)
        {
            Console.WriteLine($"Test {testName}. Iterations count: {iterationsCount}. RPS: {rps}");
            Console.WriteLine("Press any key to start...");
            Console.ReadKey();
            Console.CursorTop -= 1;
            Console.WriteLine(new string(' ', 79));
            Console.CursorTop -= 1;

            _number = 1;
            var rpsPool = new RpsPool(rps);
            var tasks = new Task[threads];
            var rnd = new Random(DateTime.UtcNow.Millisecond);
            while (threads > 0)
            {
                var curIter = iterationsCount / threads;
                iterationsCount -= curIter;

                tasks[--threads] = Task.Run(() =>
                {
                    var sw = new Stopwatch();
                    while (curIter-- > 0)
                    {
                        var dt = DateTime.UtcNow;
                        sw.Start();
                        var res = rpsPool.CanExecute();
                        sw.Stop();
                        Print($"{dt.ToString("T"),8}.{dt.Millisecond,-3} - {res,-5} | Elapsed {sw.ElapsedTicks} ticks");
                        if (minRnd <= maxRnd && maxRnd > 0)
                            Thread.Sleep(rnd.Next(minRnd, maxRnd + 1));
                        sw.Reset();
                    }
                });
            }
            Task.WaitAll(tasks);
            Console.WriteLine("Test completed. Press any key to continue...");
            Console.ReadKey();
        }

        static void Print(string str)
        {
            lock (_lockObj)
            {
                Console.WriteLine($"{_number++, 6}. {str}");
            }
        }
    }
}
