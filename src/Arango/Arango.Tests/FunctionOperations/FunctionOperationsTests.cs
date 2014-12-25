using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Arango.Client;

namespace Arango.Tests
{
    [TestFixture()]
    public class FunctionOperationsTests : IDisposable
    {
        [Test()]
        public void Should_register_function()
        {
        	Database.CreateTestDatabase(Database.TestDatabaseGeneral);
            var db = new ArangoDatabase(Database.Alias);
            
            var registerResult = db.Function.Register(
                "myfunctions::temperature::celsiustofahrenheit", 
                "function (celsius) { return celsius * 1.8 + 32; }"
            );
            
            Assert.AreEqual(201, registerResult.StatusCode);
            Assert.AreEqual(true, registerResult.Success);
            Assert.AreEqual(true, registerResult.Value);
            
            const int celsius = 30;
            const float fahrenheit = celsius * 1.8f + 32;
            
            var queryResult = db.Query
                .BindVar("celsius", celsius)
                .Aql("return myfunctions::temperature::celsiustofahrenheit(@celsius)")
                .ToList();
            
            Assert.AreEqual(fahrenheit, Convert.ToSingle(queryResult.Value.First()));
        }
        
        [Test()]
        public void Should_unregister_function()
        {
            Database.CreateTestDatabase(Database.TestDatabaseGeneral);
            var db = new ArangoDatabase(Database.Alias);
            
            var registerResult = db.Function.Register(
                "myfunctions::temperature::celsiustofahrenheit", 
                "function (celsius) { return celsius * 1.8 + 40; }"
            );
            
            Assert.AreEqual(201, registerResult.StatusCode);
            Assert.AreEqual(true, registerResult.Success);
            Assert.AreEqual(true, registerResult.Value);
            
            var unregisterResult = db.Function.Unregister("myfunctions::temperature::celsiustofahrenheit");
            
            Assert.AreEqual(200, unregisterResult.StatusCode);
            Assert.AreEqual(true, unregisterResult.Success);
            Assert.AreEqual(true, unregisterResult.Value);
        }
        
        [Test()]
        public void Should_replace_function()
        {
            Database.CreateTestDatabase(Database.TestDatabaseGeneral);
            var db = new ArangoDatabase(Database.Alias);
            
            var registerResult = db.Function.Register(
                "myfunctions::temperature::celsiustofahrenheit", 
                "function (celsius) { return celsius * 1.8 + 40; }"
            );
            
            Assert.AreEqual(201, registerResult.StatusCode);
            Assert.AreEqual(true, registerResult.Success);
            Assert.AreEqual(true, registerResult.Value);
            
            var replaceResult = db.Function.Register(
                "myfunctions::temperature::celsiustofahrenheit", 
                "function (celsius) { return celsius * 1.8 + 32; }"
            );
            
            Assert.AreEqual(200, replaceResult.StatusCode);
            Assert.AreEqual(true, replaceResult.Success);
            Assert.AreEqual(true, replaceResult.Value);
            
            const int celsius = 30;
            const float fahrenheit = celsius * 1.8f + 32;
            
            var queryResult = db.Query
                .BindVar("celsius", celsius)
                .Aql("return myfunctions::temperature::celsiustofahrenheit(@celsius)")
                .ToList();
            
            Assert.AreEqual(fahrenheit, Convert.ToSingle(queryResult.Value.First()));
        }
        
        [Test()]
        public void Should_list_functions()
        {
            Database.CreateTestDatabase(Database.TestDatabaseGeneral);
            var db = new ArangoDatabase(Database.Alias);
            
            const string name1 = "myfunctions::temperature::celsiustofahrenheit1";
            const string code1 = "function (celsius) { return celsius * 1.8 + 40; }";
            var registerResult1 = db.Function.Register(name1, code1);
            
            Assert.AreEqual(true, registerResult1.Success);
            
            const string name2 = "myfunctions::temperature::celsiustofahrenheit2";
            const string code2 = "function (celsius) { return celsius * 1.8 + 32; }";
            var registerResult2 = db.Function.Register(name2, code2);
            
            Assert.AreEqual(true, registerResult2.Success);
            
            var listResult = db.Function.List();
            
            Assert.AreEqual(200, listResult.StatusCode);
            Assert.AreEqual(true, listResult.Success);
            Assert.AreEqual(2, listResult.Value.Count);
            Assert.AreEqual(name1, listResult.Value[0].String("name"));
            Assert.AreEqual(code1, listResult.Value[0].String("code"));
            Assert.AreEqual(name2, listResult.Value[1].String("name"));
            Assert.AreEqual(code2, listResult.Value[1].String("code"));
        }
        
        public void Dispose()
        {
            Database.DeleteTestDatabase(Database.TestDatabaseGeneral);
        }
    }
}
