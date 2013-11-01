using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Arango.Client;

namespace Arango.Tests.FunctionTests
{
    [TestFixture()]
    public class ArangoFunctionTests : IDisposable
    {
        public ArangoFunctionTests()
        {
            Database.CreateTestDatabase(Database.TestDatabaseGeneral);
        }
        
        [Test()]
        public void Should_create_and_execute_function()
        {
            var db = Database.GetTestDatabase();
            
            var created = db.Function.Create(
                "myfunctions::temperature::celsiustofahrenheit", 
                "function (celsius) { return celsius * 1.8 + 32; }"
            );
            
            Assert.AreEqual(true, created);
            
            var celsius = 30;
            var fahrenheit = celsius * 1.8f + 32;
            var returnedFahrenheit = db.Query
                .Aql("return myfunctions::temperature::celsiustofahrenheit(@celsius)")
                .AddParameter("celsius", celsius)
                .ToObject<float>();
            
            Assert.AreEqual(fahrenheit, returnedFahrenheit);
        }
        
        [Test()]
        public void Should_create_replace_and_execute_function()
        {
            var db = Database.GetTestDatabase();
            
            var created = db.Function.Create(
                "myfunctions::temperature::celsiustofahrenheit", 
                "function (celsius) { return celsius * 1.8 + 40; }"
            );
            
            Assert.AreEqual(true, created);
            
            var replaced = db.Function.Replace(
                "myfunctions::temperature::celsiustofahrenheit", 
                "function (celsius) { return celsius * 1.8 + 32; }"
            );
            
            Assert.AreEqual(true, replaced);
            
            var celsius = 30;
            var fahrenheit = celsius * 1.8f + 32;
            var returnedFahrenheit = db.Query
                .Aql("return myfunctions::temperature::celsiustofahrenheit(@celsius)")
                .AddParameter("celsius", celsius)
                .ToObject<float>();
            
            Assert.AreEqual(fahrenheit, returnedFahrenheit);
        }
        
        public void Dispose()
        {
            Database.DeleteTestCollection(Database.TestDocumentCollectionName);
            Database.DeleteTestCollection(Database.TestEdgeCollectionName);
            Database.DeleteTestDatabase(Database.TestDatabaseGeneral);
        }
    }
}
