using System;
using System.Collections.Generic;
using NUnit.Framework;
using Arango.Client;

namespace Arango.Tests.DocumentTests
{
    [TestFixture()]
    public class DeserializationTests
    {
        [Test()]
        public void Should_deserialize_null()
        {
            var json = "{\"null\":null,\"embedded\":{\"null\":null}}";
            var document = new Document(json);

            // check for fields existence
            Assert.AreEqual(true, document.HasField("null"));
            Assert.AreEqual(true, document.HasField("embedded"));
            Assert.AreEqual(true, document.HasField("embedded.null"));

            // check for fields values
            Assert.AreEqual(null, document.GetField<object>("null"));
            Assert.AreEqual(null, document.GetField<object>("embedded.null"));
            
            // check generated json value
            Assert.AreEqual(json, Document.Serialize(document));
        }

        [Test()]
        public void Should_deserialize_boolean()
        {
            var json = "{\"isTrue\":true,\"isFalse\":false,\"embedded\":{\"isTrue\":true,\"isFalse\":false},\"array\":[true,false]}";
            var document = new Document(json);

            // check for fields existence
            Assert.AreEqual(true, document.HasField("isTrue"));
            Assert.AreEqual(true, document.HasField("isFalse"));
            Assert.AreEqual(true, document.HasField("embedded"));
            Assert.AreEqual(true, document.HasField("embedded.isTrue"));
            Assert.AreEqual(true, document.HasField("embedded.isFalse"));
            Assert.AreEqual(true, document.HasField("array"));
            
            // check for fields values
            Assert.AreEqual(true, document.GetField<bool>("isTrue"));
            Assert.AreEqual(false, document.GetField<bool>("isFalse"));
            Assert.AreEqual(true, document.GetField<bool>("embedded.isTrue"));
            Assert.AreEqual(false, document.GetField<bool>("embedded.isFalse"));
            
            List<bool> array = document.GetField<List<bool>>("array");
            Assert.AreEqual(2, array.Count);
            Assert.AreEqual(true, array[0]);
            Assert.AreEqual(false, array[1]);
            
            // check generated json value
            Assert.AreEqual(json, Document.Serialize(document));
        }

        [Test()]
        public void Should_deserialize_numbers()
        {
            var json = "{\"integer\":123,\"float\":3.14,\"embedded\":{\"integer\":123,\"float\":3.14},\"intArray\":[123,456],\"floatArray\":[2.34,4.567]}";
            var document = new Document(json);
            
            // check for fields existence
            Assert.AreEqual(true, document.HasField("integer"));
            Assert.AreEqual(true, document.HasField("float"));
            Assert.AreEqual(true, document.HasField("embedded"));
            Assert.AreEqual(true, document.HasField("embedded.integer"));
            Assert.AreEqual(true, document.HasField("embedded.float"));
            Assert.AreEqual(true, document.HasField("intArray"));
            Assert.AreEqual(true, document.HasField("floatArray"));
            
            // check for fields values
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
            
            // check generated json value
            Assert.AreEqual(json, Document.Serialize(document));
        }

        [Test()]
        public void Should_deserialize_strings()
        {
            var json = "{\"string\":\"foo bar\",\"embedded\":{\"string\":\"foo bar\",\"array\":[\"foo\",\"bar\"]},\"array\":[\"foo\",\"bar\"]}";
            var document = new Document(json);
            
            // check for fields existence
            Assert.AreEqual(true, document.HasField("string"));
            Assert.AreEqual(true, document.HasField("embedded"));
            Assert.AreEqual(true, document.HasField("embedded.string"));
            Assert.AreEqual(true, document.HasField("embedded.array"));
            Assert.AreEqual(true, document.HasField("array"));
            
            // check for fields values
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
            
            // check generated json value
            Assert.AreEqual(json, Document.Serialize(document));
        }
        
        [Test()]
        public void Should_deserialize_datetime_as_objects()
        {
            ArangoClient.Settings.DeserializeDateTimeAsString = true;
            var dateTime = DateTime.Parse("2008-12-20T02:12:02");
            
            var json = "{\"datetime1\":\"2008-12-20T02:12:02Z\",\"datetime2\":\"" + dateTime.ToString("yyyy-MM-dd'T'HH:mm:ss") + "\"}";
            var document = Document.Deserialize(json, false);
            
            // check if the fields existence
            Assert.AreEqual(true, document.HasField("datetime1"));
            Assert.AreEqual(true, document.HasField("datetime2"));
            
            // check for fields values
            Assert.AreEqual(TimeZoneInfo.ConvertTimeToUtc(DateTime.Parse("2008-12-20T02:12:02Z")), document.GetField<DateTime>("datetime1"));
            Assert.AreEqual(dateTime, document.GetField<DateTime>("datetime2"));
        }
        
        [Test()]
        public void Should_deserialize_datetime_as_strings()
        {
            ArangoClient.Settings.DeserializeDateTimeAsString = true;
            var dateTime = DateTime.Parse("2008-12-20T02:12:02");
            
            var json = "{\"datetime1\":\"2008-12-20T02:12:02Z\",\"datetime2\":\"" + dateTime.ToString("yyyy-MM-dd'T'HH:mm:ss") + "\"}";
            var document = Document.Deserialize(json);
            
            // check if the fields existence
            Assert.AreEqual(true, document.HasField("datetime1"));
            Assert.AreEqual(true, document.HasField("datetime2"));
            
            // check for fields values
            Assert.AreEqual("2008-12-20T02:12:02Z", document.GetField<string>("datetime1"));
            Assert.AreEqual(dateTime.ToString("yyyy-MM-dd'T'HH:mm:ss"), document.GetField<string>("datetime2"));
        }
    }
}

