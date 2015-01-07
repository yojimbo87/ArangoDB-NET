using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Arango.Client;

namespace Arango.Tests
{
    [TestFixture()]
    public class ConnectionlessOperationsTests
    {
        [Test()]
        public void Should_store_and_retrieve_basic_arango_specific_fields_from_document()
        {
            var doc = new Dictionary<string, object>()
                .ID("col/123")
                .Key("123")
                .Rev("123456789");
            
            Assert.IsTrue(doc.HasID());
            Assert.AreEqual("col/123", doc.ID());
            Assert.IsTrue(doc.HasKey());
            Assert.AreEqual("123", doc.Key());
            Assert.IsTrue(doc.HasRev());
            Assert.AreEqual("123456789", doc.Rev());
        }
    }
}
