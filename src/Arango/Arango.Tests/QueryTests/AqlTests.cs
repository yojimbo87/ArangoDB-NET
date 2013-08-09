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
        [Test()]
        public void Should_generate_simple_query()
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
    }
}
