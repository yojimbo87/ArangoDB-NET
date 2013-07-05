using System.Collections.Generic;
using NUnit.Framework;
using Arango.Client;

namespace Arango.Tests.DocumentTests
{
    [TestFixture()]
    public class DocumentTests
    {
        [Test()]
        public void Should_remove_fields_()
        {
            // setup document with some fields
            Document document = new Document()
                .SetField("foo", "string value 1")
                .SetField("bar", "string value 2")
                .SetField("baz.foo", "string value 3");
            
            // check if the fields are present
            Assert.AreEqual(true, document.HasField("foo"));
            Assert.AreEqual(true, document.HasField("bar"));
            Assert.AreEqual(true, document.HasField("baz.foo"));
            
            // remove some fields
            document.RemoveField("bar");
            document.RemoveField("baz.foo");
            
            // check if the fields were removed
            Assert.AreEqual(true, document.HasField("foo"));
            Assert.AreEqual(false, document.HasField("bar"));
            Assert.AreEqual(false, document.HasField("baz.foo"));
        }
    }
}
