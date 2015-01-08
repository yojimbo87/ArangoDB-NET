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
        public void Should_validate_standard_field_format()
        {
            // _id validation
            Assert.IsTrue(ADocument.IsID("col/123"));
            Assert.IsFalse(ADocument.IsID("/123"));
            Assert.IsFalse(ADocument.IsID("col/"));
            Assert.IsFalse(ADocument.IsID("col/123/111"));
            Assert.IsFalse(ADocument.IsID("col/123a"));
            
            // _key validation
            Assert.IsTrue(ADocument.IsKey("123"));
            Assert.IsFalse(ADocument.IsKey("123a"));
            
            // _rev validation
            Assert.IsTrue(ADocument.IsRev("123"));
            Assert.IsFalse(ADocument.IsRev("123a"));
        }
        
        [Test()]
        public void Should_construct_document_IDs()
        {
            Assert.AreEqual("col/123", ADocument.Identify("col", 123));
            Assert.AreEqual("col/123", ADocument.Identify("col", "123"));
            
            Assert.AreEqual(null, ADocument.Identify("col", "123a"));
        }
        
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
            
            Assert.Throws<ArgumentException>(() => {
                 var doc1 = new Dictionary<string, object>()
                     .ID("col/123a");
            });
            
            Assert.Throws<ArgumentException>(() => {
                 var doc1 = new Dictionary<string, object>()
                     .Key("123a");
            });
            
            Assert.Throws<ArgumentException>(() => {
                 var doc1 = new Dictionary<string, object>()
                     .Rev("123a");
            });
        }
    }
}
