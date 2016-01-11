using System;
using Arango.Client;

namespace Arango.ConsoleTests
{
    public static class AqlTests
    {
        public static void TestAqlGenerator()
        {
            var myQueries = new MyQueries();
            
            var testCall = myQueries.Call("test");
            
            Console.WriteLine(testCall);
            
            var otherCall = myQueries.Call("other");
            
            Console.WriteLine(otherCall);
        }
    }
    
    public class MyQueries : AQL
    {
        public MyQueries()
        {
            Queries["test"] = () =>
            {
                FOR("some string which belongs to test query");
            };
            
            Queries["other"] = () =>
            {
                FOR("some string which belongs to other query");
            };
        }
    }
}
