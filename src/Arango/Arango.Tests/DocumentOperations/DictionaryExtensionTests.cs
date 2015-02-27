using System.Collections.Generic;
using Arango.Client;
using NUnit.Framework;

namespace Arango.Tests
{
    public class NullableFieldTestClass
    {
        public int? TestField { get; set; }
    }

    [TestFixture]
    public class DictionaryExtensionTests
    {
        [Test]
        public void To_object_with_nullable_field_set_as_null()
        {
            var dict = new Dictionary<string, object>();
            dict.Add("TestField", null);

            var res = dict.ToObject<NullableFieldTestClass>();

            Assert.IsNull(res.TestField);
        }

        [Test]
        public void To_object_with_nullable_field_set_with_value()
        {
            var dict = new Dictionary<string, object>();
            dict.Add("TestField", 64);

            var res = dict.ToObject<NullableFieldTestClass>();

            Assert.AreEqual(64, res.TestField);
        }
    }
}