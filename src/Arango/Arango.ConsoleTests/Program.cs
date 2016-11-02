using System;

namespace Arango.ConsoleTests
{
    class Program
    {
        public static void Main(string[] args)
        {
            //PerformanceTests();
            FoxxPerformanceTests();
            
            Console.Write("Press any key to continue . . . ");
            Console.ReadKey(true);
        }

        static void PerformanceTests()
        {
            var iterationCount = 10000;
            var performance = new Performance();
            
            performance.TestArangoClientSequentialInsertion(iterationCount);
            performance.TestSimpleSequentialHttpPostRequests(iterationCount);

            //performance.TestRestSharpHttpPostRequests();
            //performance.TestSimpleParallelHttpPostRequests();

            performance.Dispose();
        }

        static void FoxxPerformanceTests()
        {
            var iterationCount = 10000;
            var performance = new FoxxPerformance();

            performance.TestPostCall(iterationCount);
        }
    }
}