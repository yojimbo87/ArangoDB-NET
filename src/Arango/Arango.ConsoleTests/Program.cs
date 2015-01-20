using System;
using System.Collections.Generic;
using Arango.Client;
using Arango.fastJSON;

namespace Arango.ConsoleTests
{
    class Program
    {
        public static void Main(string[] args)
        {
            //PerformanceTests();
            
            Console.Write("Press any key to continue . . . ");
            Console.ReadKey(true);
        }
        
        static void PerformanceTests()
        {
            var performance = new Performance();
            
            //performance.TestSimpleSequentialHttpPostRequests();
            //performance.TestRestSharpHttpPostRequests();
            //performance.TestSimpleParallelHttpPostRequests();
            
            performance.Dispose();
        }
    }
}