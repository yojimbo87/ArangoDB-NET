using System;
using System.Collections.Generic;
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
            Assert.IsTrue(ADocument.IsID("col/123a"));
            Assert.IsTrue(ADocument.IsID("col/a123-a"));
            Assert.IsTrue(ADocument.IsID("col/a123:a"));
            Assert.IsTrue(ADocument.IsID("col/a123_a"));
            Assert.IsTrue(ADocument.IsID("col/a123_a:b-c"));
            Assert.IsTrue(ADocument.IsID("col/xYz09_-:.@()+,=;$!*'%"));
            Assert.IsFalse(ADocument.IsID("/123"));
            Assert.IsFalse(ADocument.IsID("col/"));
            Assert.IsFalse(ADocument.IsID("col/123/111"));
            Assert.IsFalse(ADocument.IsID("col/123 111"));
            Assert.IsFalse(ADocument.IsID(""));
            
            // _key validation
            Assert.IsTrue(ADocument.IsKey("123"));
            Assert.IsTrue(ADocument.IsKey("123a"));
            Assert.IsTrue(ADocument.IsKey("a123-a"));
            Assert.IsTrue(ADocument.IsKey("a123:a"));
            Assert.IsTrue(ADocument.IsKey("a123_a"));
            Assert.IsTrue(ADocument.IsKey("a123_a:b-c"));
            Assert.IsTrue(ADocument.IsKey("xYz09_-:.@()+,=;$!*'%"));
            Assert.IsFalse(ADocument.IsKey("123/111"));
            Assert.IsFalse(ADocument.IsKey("123 111"));
            Assert.IsFalse(ADocument.IsKey("a123_a :b-c"));
            Assert.IsFalse(ADocument.IsKey(""));
            
            // _rev validation
            Assert.IsTrue(ADocument.IsRev("123"));
            Assert.IsTrue(ADocument.IsRev("123aBc-"));
            Assert.IsFalse(ADocument.IsRev(""));
        }
        
        [Test()]
        public void Should_validate_document_ID_fields()
        {
            var doc1 = new Dictionary<string, object>()
                .String("id1", "myCollection/123")
                .String("id2", "myCollection/a123-4:5_6")
                .String("id3", "myCollection/xYz09_-:.@()+,=;$!*'%")
                .Long("id4", 123)
                .Object("id5", null)
                .String("id6", "");
            
            Assert.IsTrue(doc1.IsID("id1"));
            Assert.IsTrue(doc1.IsID("id2"));
            Assert.IsTrue(doc1.IsID("id3"));

            Assert.IsFalse(doc1.IsID("id4"));
            Assert.IsFalse(doc1.IsID("id5"));
            Assert.IsFalse(doc1.IsID("id6"));
            Assert.IsFalse(doc1.IsID("nonExistingField"));
        }
        
        [Test()]
        public void Should_validate_document_key_fields()
        {
            var doc1 = new Dictionary<string, object>()
                .String("key1", "123")
                .String("key2", "a123-4:5_6")
                .String("key3", "xYz09_-:.@()+,=;$!*'%")
                .Long("key4", 123)
                .Object("key5", null)
                .String("key6", "");
            
            Assert.IsTrue(doc1.IsKey("key1"));
            Assert.IsTrue(doc1.IsKey("key2"));
            Assert.IsTrue(doc1.IsKey("key3"));

            Assert.IsFalse(doc1.IsKey("key4"));
            Assert.IsFalse(doc1.IsKey("key5"));
            Assert.IsFalse(doc1.IsKey("key6"));
            Assert.IsFalse(doc1.IsKey("nonExistingField"));
        }
        
        [Test()]
        public void Should_construct_document_IDs()
        {
            Assert.AreEqual("col/123", ADocument.Identify("col", 123));
            Assert.AreEqual("col/123", ADocument.Identify("col", "123"));
            Assert.AreEqual("col/123a", ADocument.Identify("col", "123a"));
            
            Assert.AreEqual(null, ADocument.Identify("col", "123 a"));
            Assert.AreEqual(null, ADocument.Identify("", "123"));
            Assert.AreEqual(null, ADocument.Identify("col", ""));
        }
        
        [Test()]
        public void Should_parse_keys_from_document_IDs()
        {
            Assert.AreEqual("123", ADocument.ParseKey("col/123"));
            Assert.AreEqual("a123-4:5_6", ADocument.ParseKey("col/a123-4:5_6"));
            
            Assert.AreEqual(null, ADocument.ParseKey("col/123 a"));
            Assert.AreEqual(null, ADocument.ParseKey("/123"));
            Assert.AreEqual(null, ADocument.ParseKey("col/"));
        }
        
        [Test()]
        public void Should_store_and_retrieve_basic_arango_specific_fields_from_document()
        {
            var doc = new Dictionary<string, object>()
                .ID("col/123_a:b-c")
                .Key("123_a:b-c")
                .Rev("123456789")
                .From("col2/456_d:e-f")
                .To("col3/789_g:h-i");
            
            Assert.IsTrue(doc.HasID());
            Assert.AreEqual("col/123_a:b-c", doc.ID());
            Assert.IsTrue(doc.HasKey());
            Assert.AreEqual("123_a:b-c", doc.Key());
            Assert.IsTrue(doc.HasRev());
            Assert.AreEqual("123456789", doc.Rev());
            Assert.IsTrue(doc.HasFrom());
            Assert.AreEqual("col2/456_d:e-f", doc.From());
            Assert.IsTrue(doc.HasTo());
            Assert.AreEqual("col3/789_g:h-i", doc.To());

            Assert.Throws<ArgumentException>(() => {
                 var doc1 = new Dictionary<string, object>()
                     .ID("col/123 a");
            });
            
            Assert.Throws<ArgumentException>(() => {
                 var doc1 = new Dictionary<string, object>()
                     .Key("123 a");
            });
            
            Assert.Throws<ArgumentException>(() => {
                 var doc1 = new Dictionary<string, object>()
                     .Rev("");
            });

            Assert.Throws<ArgumentException>(() => {
                var doc1 = new Dictionary<string, object>()
                    .From("col2/456 a");
            });

            Assert.Throws<ArgumentException>(() => {
                var doc1 = new Dictionary<string, object>()
                    .To("col3/789 a");
            });
        }
    }
}
