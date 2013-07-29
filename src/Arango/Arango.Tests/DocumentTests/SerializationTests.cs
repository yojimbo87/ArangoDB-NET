using System;
using System.Collections.Generic;
using System.Globalization;
using NUnit.Framework;
using Arango.Client;

namespace Arango.Tests.DocumentTests
{
    [TestFixture()]
    public class SerializationTests
    {
        [Test()]
        public void Should_serialize_null()
        {
            // fill document with data
            var document = new Document()
                .SetField<object>("null", null)
                .SetField<object>("embedded.null", null);

            // check if document data types are equal on retrieval
            Assert.AreEqual(null, document.GetField<object>("null"));
            Assert.AreEqual(null, document.GetField<object>("embedded.null"));
            
            // compare json representation of document
            var expected = "{\"null\":null,\"embedded\":{\"null\":null}}";
            var actual = Document.Serialize(document);

            Assert.AreEqual(expected, actual);
        }

        [Test()]
        public void Should_serialize_boolean()
        {
            // fill document with data
            var document = new Document()
                .SetField("isTrue", true)
                .SetField("isFalse", false)
                .SetField("embedded.isTrue", true)
                .SetField("embedded.isFalse", false)
                .SetField<List<bool>>("array", new List<bool> { true, false });

            // check if document data types are equal on retrieval
            Assert.AreEqual(true, document.GetField<bool>("isTrue"));
            Assert.AreEqual(false, document.GetField<bool>("isFalse"));
            Assert.AreEqual(true, document.GetField<bool>("embedded.isTrue"));
            Assert.AreEqual(false, document.GetField<bool>("embedded.isFalse"));
            
            List<bool> array = document.GetField<List<bool>>("array");
            Assert.AreEqual(2, array.Count);
            Assert.AreEqual(true, array[0]);
            Assert.AreEqual(false, array[1]);
            
            // compare json representation of document
            var expected = "{\"isTrue\":true,\"isFalse\":false,\"embedded\":{\"isTrue\":true,\"isFalse\":false},\"array\":[true,false]}";
            var actual = Document.Serialize(document);

            Assert.AreEqual(expected, actual);
        }

        [Test()]
        public void Should_serialize_numbers()
        {
            // fill document with data
            var document = new Document()
                .SetField("integer", int.Parse("123"))
                .SetField("float", float.Parse("3.14", CultureInfo.InvariantCulture))
                .SetField("embedded.integer", int.Parse("123"))
                .SetField("embedded.float", float.Parse("3.14", CultureInfo.InvariantCulture))
                .SetField("intArray", new List<int> {123, 456})
                .SetField("floatArray", new List<float> {2.34f, 4.567f});

            // check if document data types are equal on retrieval
            Assert.AreEqual(123, document.GetField<int>("integer"));
            Assert.AreEqual(3.14f, document.GetField<float>("float"));
            Assert.AreEqual(123, document.GetField<int>("embedded.integer"));
            Assert.AreEqual(3.14f, document.GetField<float>("embedded.float"));
            
            List<int> intArray = document.GetField<List<int>>("intArray");
            Assert.AreEqual(2, intArray.Count);
            Assert.AreEqual(123, intArray[0]);
            Assert.AreEqual(456, intArray[1]);
            
            List<float> floatArray = document.GetField<List<float>>("floatArray");
            Assert.AreEqual(2, floatArray.Count);
            Assert.AreEqual(2.34f, floatArray[0]);
            Assert.AreEqual(4.567f, floatArray[1]);
            
            // compare json representation of document
            var expected = "{\"integer\":123,\"float\":3.14,\"embedded\":{\"integer\":123,\"float\":3.14},\"intArray\":[123,456],\"floatArray\":[2.34,4.567]}";
            var actual = Document.Serialize(document);

            Assert.AreEqual(expected, actual);
        }

        [Test()]
        public void Should_serialize_strings()
        {
            // fill document with data
            var document = new Document()
                .SetField("string", "foo bar")
                .SetField("embedded.string", "foo bar")
                .SetField("embedded.array", new List<string> { "foo", "bar" })
                .SetField("array", new List<string> { "foo", "bar" });

            // check if document data types are equal on retrieval
            Assert.AreEqual("foo bar", document.GetField<string>("string"));
            Assert.AreEqual("foo bar", document.GetField<string>("embedded.string"));
            
            List<string> array = document.GetField<List<string>>("embedded.array");
            Assert.AreEqual(2, array.Count);
            Assert.AreEqual("foo", array[0]);
            Assert.AreEqual("bar", array[1]);
            
            array = document.GetField<List<string>>("array");
            Assert.AreEqual(2, array.Count);
            Assert.AreEqual("foo", array[0]);
            Assert.AreEqual("bar", array[1]);
            
            // compare json representation of document
            var expected = "{\"string\":\"foo bar\",\"embedded\":{\"string\":\"foo bar\",\"array\":[\"foo\",\"bar\"]},\"array\":[\"foo\",\"bar\"]}";
            var actual = Document.Serialize(document);

            Assert.AreEqual(expected, actual);
        }
        
        [Test()]
        public void Should_serialize_datetime()
        {
            var dateTime = new DateTime(2008, 12, 20, 2, 12, 2);
            
            // fill document with data
            var document = new Document()
                .SetField("datetime1", "2008-12-20T02:12:02Z")
                .SetField("datetime2", dateTime)
                .SetField("datetime3", TimeZoneInfo.ConvertTimeToUtc(DateTime.Parse("2008-12-20T02:12:02Z")));

            // check if document data types are equal on retrieval
            Assert.AreEqual("2008-12-20T02:12:02Z", document.GetField<string>("datetime1"));
            Assert.AreEqual(dateTime, document.GetField<DateTime>("datetime2"));
            Assert.AreEqual(TimeZoneInfo.ConvertTimeToUtc(DateTime.Parse("2008-12-20T02:12:02Z")), document.GetField<DateTime>("datetime3"));
            
            // compare json representation of document
            var expected = "{\"datetime1\":\"2008-12-20T02:12:02Z\",\"datetime2\":\"2008-12-20T02:12:02\",\"datetime3\":\"2008-12-20T02:12:02Z\"}";
            var actual = Document.Serialize(document);

            Assert.AreEqual(expected, actual);
        }
    }
}

