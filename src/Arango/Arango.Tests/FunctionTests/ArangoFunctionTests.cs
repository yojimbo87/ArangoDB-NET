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
            var name = "myfunctions::temperature::celsiustofahrenheit";
            
            var created = db.Function.Create(
                name, 
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
            
            db.Function.Delete(name);
        }
        
        [Test()]
        public void Should_create_replace_and_execute_function()
        {
            var db = Database.GetTestDatabase();
            var name = "myfunctions::temperature::celsiustofahrenheit";
            
            var created = db.Function.Create(
                name, 
                "function (celsius) { return celsius * 1.8 + 40; }"
            );
            
            Assert.AreEqual(true, created);
            
            var replaced = db.Function.Replace(
                name, 
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
            
            db.Function.Delete(name);
        }
        
        [Test()]
        public void Should_create_and_delete_function()
        {
            var db = Database.GetTestDatabase();
            
            var created = db.Function.Create(
                "myfunctions::temperature::celsiustofahrenheit", 
                "function (celsius) { return celsius * 1.8 + 40; }"
            );
            
            Assert.AreEqual(true, created);
            
            var deleted = db.Function.Delete("myfunctions::temperature::celsiustofahrenheit");
            
            Assert.AreEqual(true, deleted);
        }
        
        [Test()]
        public void Should_create_and_get_functions()
        {
            var db = Database.GetTestDatabase();
            
            var name1 = "myfunctions::temperature::celsiustofahrenheit1";
            var code1 = "function (celsius) { return celsius * 1.8 + 40; }";
            var created1 = db.Function.Create(name1, code1);
            
            Assert.AreEqual(true, created1);
            
            var name2 = "myfunctions::temperature::celsiustofahrenheit2";
            var code2 = "function (celsius) { return celsius * 1.8 + 32; }";
            var created2 = db.Function.Create(name2, code2);
            
            Assert.AreEqual(true, created2);
            
            var functions = db.Function.Get();
            
            Assert.AreEqual(2, functions.Count);
            Assert.AreEqual(name1, functions[0].String("name"));
            Assert.AreEqual(code1, functions[0].String("code"));
            Assert.AreEqual(name2, functions[1].String("name"));
            Assert.AreEqual(code2, functions[1].String("code"));
        }
        
        public void Dispose()
        {
            Database.DeleteTestCollection(Database.TestDocumentCollectionName);
            Database.DeleteTestCollection(Database.TestEdgeCollectionName);
            Database.DeleteTestDatabase(Database.TestDatabaseGeneral);
        }
    }
}
