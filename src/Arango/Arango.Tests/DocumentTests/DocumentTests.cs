using System;
using System.Collections.Generic;
using NUnit.Framework;
using Arango.Client;

namespace Arango.Tests.DocumentTests
{
    [TestFixture()]
    public class DocumentTests
    {
        [Test()]
        public void Should_check_document_identifier()
        {
            var id1 = "collection";
            var id2 = "/collection";
            var id3 = "collection/";
            var id4 = "collection/a";
            var id5 = "collection/1";
            
            Assert.AreEqual(false, Document.IsId(id1));
            Assert.AreEqual(false, Document.IsId(id2));
            Assert.AreEqual(false, Document.IsId(id3));
            Assert.AreEqual(false, Document.IsId(id4));
            Assert.AreEqual(true, Document.IsId(id5));
        }
    }
}
