using System;
using System.Collections.Generic;
using System.Diagnostics;
using Arango.Client.ExternalLibraries.dictator;
using Arango.Client.Public;

namespace Arango.ConsoleTests
{
    public class FoxxPerformance
    {
        private readonly Stopwatch _stopWatch = new Stopwatch();
        private readonly ADatabase _db = new ADatabase(Database.SystemAlias);

        public void TestPostCall(int iterationCount)
        {
            var body = Dictator.New()
                .String("foo", "some string");

            /*_stopWatch.Start();

            var postResult = _db.Foxx
                .Body(body)
                .Post<Dictionary<string, object>>("/getting-started/hello-world");

            _stopWatch.Stop();

            var ts = _stopWatch.Elapsed;
            var elapsedTime = string.Format(
                "{0:00}:{1:00}:{2:00}.{3:00}",
                ts.Hours,
                ts.Minutes,
                ts.Seconds,
                ts.Milliseconds / 10
            );

            Console.WriteLine("RunTime " + elapsedTime);*/

            Console.WriteLine("Operation start: TestPostCall\n");

            ExecuteTimedTest(iterationCount, () => {
                var postResult = _db.Foxx
                    .Body(body)
                    .Post<Dictionary<string, object>>("/getting-started/hello-world");
            });

            Console.WriteLine("\nOperation end: TestPostCall");
        }

        static void ExecuteTimedTest(int iterations, Action test)
        {
            double jit = Execute(test); //disregard jit pass
            Console.WriteLine("JIT: {0:F2}ms.", jit);

            double optimize = Execute(test); //disregard optimize pass
            Console.WriteLine("Optimize: {0:F2}ms.", optimize);

            double totalElapsed = 0;

            for (int i = 0; i < iterations; i++)
            {
                totalElapsed += Execute(test);
            }

            double averageMs = (totalElapsed / iterations);

            Console.WriteLine("Total: {0:F2}ms.", totalElapsed);
            Console.WriteLine("Average: {0:F2}ms.", averageMs);
        }

        static double Execute(Action action)
        {
            var stopwatch = Stopwatch.StartNew();

            action();

            return stopwatch.Elapsed.TotalMilliseconds;
        }
    }
}
