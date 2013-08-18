using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Arango.Client;

namespace Arango.Tests.QueryTests
{
    [TestFixture()]
    public class AqlTests
    {
        /*[Test()]
        public void Should_generate_for_filter_return_query()
        {
            var query = 
                "FOR f IN Foo " +
                    "FILTER f.bar == 'xyz' " +
                    "RETURN f";
            
            var generatedQuery = new ArangoQueryOperation()
                .For("f", "Foo")
                    .Filter("f.bar").Equals("xyz")
                    .Return("f")
                .ToString();
            
            Assert.AreEqual(query, generatedQuery);
        }
        
        [Test()]
        public void Should_generate_let_for_filter_return_query()
        {
            var query = 
                "LET foo = (" +
                    "FOR f IN Foo " +
                        "FILTER f.bar == 'xyz' " +
                        "RETURN f.bar" +
                    ") " +
                "FOR f IN Foo " +
                    "FILTER f.bar == foo " +
                    "RETURN f";
            
            var generatedQuery = new ArangoQueryOperation()
                .Let("foo")
                    .For("f", "Foo")
                        .Filter("f.bar").Equals("xyz")
                        .Return("f.bar")
                .For("f", "Foo")
                    .Filter("f.bar").Equals("foo")
                    .Return("f")
                .ToString();
            
            Assert.AreEqual(query, generatedQuery);
        }*/
    }
}
