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
        public void Should_check_presence_of_non_existing_field()
        {
            // setup document with some fields
            var document = new Document()
                .SetField("foo", "string value 1")
                .SetField("bar", "string value 2")
                .SetField("baz.foo", "string value 3")
                .SetField<object>("baz.null", null);

            // check if the fields were removed
            Assert.AreEqual(true, document.HasField("foo"));
            Assert.AreEqual(true, document.HasField("bar"));
            Assert.AreEqual(true, document.HasField("baz.foo"));
            Assert.AreEqual(true, document.HasField("baz.null"));
            Assert.AreEqual(false, document.HasField("baz.bar"));
            Assert.AreEqual(false, document.HasField("baz.bar.foo"));
            Assert.AreEqual(false, document.HasField("baz.null.foo"));
        }
        
        [Test()]
        public void Should_remove_fields()
        {
            // setup document with some fields
            var document = new Document()
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
        
        [Test()]
        public void Should_convert_to_generic_object()
        {
            // setup document with some fields
            var document = new Document()
                .SetField("FirstName", "Johny")
                .SetField("LastName", "Bravo")
                .SetField("Age", 25)
                .SetField("ShouldNotBeSerializedOrDeserialized", "shouldn't be seen")
                .SetField("aliasedField", "aliased string")
                .SetField("Interests", new List<string> {"programming", "hacking", "coding"});
            
            //var follower1 = new Person { FirstName = "Mike", LastName = "Tall", Interests = new List<string> { "biking", "skiing"} };
            //var follower2 = new Person { FirstName = "Lucy", LastName = "Fox", Interests = new List<string> { "cooking", "gardening"} };
            
            var father = new Document()
                .SetField("FirstName", "Larry")
                .SetField("LastName", "Bravo")
                .SetField("Age", 45)
                .SetField("Interests", new List<string> { "couching", "tennis" });
            
            document.SetField("Father", father);
            
            var follower1 = new Document()
                .SetField("FirstName", "Mike")
                .SetField("LastName", "Tall")
                .SetField("Interests", new List<string> { "biking", "skiing" });
            
            var follower2 = new Document()
                .SetField("FirstName", "Lucy")
                .SetField("LastName", "Fox")
                .SetField("Interests", new List<string> { "cooking", "gardening" });
            
            document.SetField("Followers", new List<Document> { follower1, follower2 });
            
            var person = document.To<Person>();
            
            // check if the object properties and document fields are equal
            Assert.AreEqual(document.GetField<string>("FirstName"), person.FirstName);
            Assert.AreEqual(document.GetField<string>("LastName"), person.LastName);
            Assert.AreEqual(document.GetField<int>("Age"), person.Age);
            Assert.AreEqual(true, string.IsNullOrEmpty(person.ShouldNotBeSerializedOrDeserialized));
            Assert.AreEqual(document.GetField<string>("aliasedField"), person.Aliased);
            
            Assert.AreEqual(document.GetField<List<string>>("Interests").Count, person.Interests.Count);
            Assert.AreEqual(document.GetField<List<string>>("Interests")[0], person.Interests[0]);
            Assert.AreEqual(document.GetField<List<string>>("Interests")[1], person.Interests[1]);
            Assert.AreEqual(document.GetField<List<string>>("Interests")[2], person.Interests[2]);
            
            Assert.AreEqual(document.GetField<string>("Father.FirstName"), person.Father.FirstName);
            Assert.AreEqual(document.GetField<string>("Father.LastName"), person.Father.LastName);
            Assert.AreEqual(document.GetField<int>("Father.Age"), person.Father.Age);
            Assert.AreEqual(document.GetField<List<string>>("Father.Interests").Count, person.Father.Interests.Count);
            Assert.AreEqual(document.GetField<List<string>>("Father.Interests")[0], person.Father.Interests[0]);
            Assert.AreEqual(document.GetField<List<string>>("Father.Interests")[1], person.Father.Interests[1]);
            
            Assert.AreEqual(document.GetField<List<Person>>("Followers").Count, person.Followers.Count);
            Assert.AreEqual(document.GetField<List<Person>>("Followers")[0].FirstName, person.Followers[0].FirstName);
            Assert.AreEqual(document.GetField<List<Person>>("Followers")[0].LastName, person.Followers[0].LastName);
            Assert.AreEqual(document.GetField<List<Person>>("Followers")[0].Interests.Count, person.Followers[0].Interests.Count);
            Assert.AreEqual(document.GetField<List<Person>>("Followers")[0].Interests[0], person.Followers[0].Interests[0]);
            Assert.AreEqual(document.GetField<List<Person>>("Followers")[0].Interests[1], person.Followers[0].Interests[1]);
            Assert.AreEqual(document.GetField<List<Person>>("Followers")[1].FirstName, person.Followers[1].FirstName);
            Assert.AreEqual(document.GetField<List<Person>>("Followers")[1].LastName, person.Followers[1].LastName);
            Assert.AreEqual(document.GetField<List<Person>>("Followers")[1].Interests[0], person.Followers[1].Interests[0]);
            Assert.AreEqual(document.GetField<List<Person>>("Followers")[1].Interests[1], person.Followers[1].Interests[1]);
        }
        
        [Test()]
        public void Should_convert_from_generic_object()
        {
            // setup object with some data
            var person = new Person();
            person.FirstName = "Johny";
            person.LastName = "Bravo";
            person.Age = 25;
            person.ShouldNotBeSerializedOrDeserialized = "shouldn't be seen";
            person.Aliased = "aliased string";
            person.Interests = new List<string> {"programming", "hacking", "coding"};
            person.Father = new Person { FirstName = "Larry", LastName = "Bravo", Age = 45, Interests = new List<string> { "couching", "tennis" } };
            
            var follower1 = new Person { FirstName = "Mike", LastName = "Tall", Interests = new List<string> { "biking", "skiing"} };
            var follower2 = new Person { FirstName = "Lucy", LastName = "Fox", Interests = new List<string> { "cooking", "gardening"} };
            
            person.Followers = new List<Person> { follower1, follower2 };
            
            var document = new Document();
            document.From(person);
            
            // check if the document fields and object properties are equal
            Assert.AreEqual(person.FirstName, document.GetField<string>("FirstName"));
            Assert.AreEqual(person.LastName, document.GetField<string>("LastName"));
            Assert.AreEqual(person.Age, document.GetField<int>("Age"));
            Assert.AreEqual(false, document.HasField("ShouldNotBeSerializedOrDeserialized"));
            Assert.AreEqual(person.Aliased, document.GetField<string>("aliasedField"));
            
            Assert.AreEqual(person.Interests.Count, document.GetField<List<string>>("Interests").Count);
            Assert.AreEqual(person.Interests[0], document.GetField<List<string>>("Interests")[0]);
            Assert.AreEqual(person.Interests[1], document.GetField<List<string>>("Interests")[1]);
            Assert.AreEqual(person.Interests[2], document.GetField<List<string>>("Interests")[2]);
            
            Assert.AreEqual(person.Father.FirstName, document.GetField<string>("Father.FirstName"));
            Assert.AreEqual(person.Father.LastName, document.GetField<string>("Father.LastName"));
            Assert.AreEqual(person.Father.Age, document.GetField<int>("Father.Age"));
            Assert.AreEqual(person.Father.Interests.Count, document.GetField<List<string>>("Father.Interests").Count);
            Assert.AreEqual(person.Father.Interests[0], document.GetField<List<string>>("Father.Interests")[0]);
            Assert.AreEqual(person.Father.Interests[1], document.GetField<List<string>>("Father.Interests")[1]);
            
            Assert.AreEqual(person.Followers.Count, document.GetField<List<Document>>("Followers").Count);
            Assert.AreEqual(person.Followers[0].FirstName, document.GetField<List<Document>>("Followers")[0].GetField<string>("FirstName"));
            Assert.AreEqual(person.Followers[0].LastName, document.GetField<List<Document>>("Followers")[0].GetField<string>("LastName"));
            Assert.AreEqual(person.Followers[0].Interests.Count, document.GetField<List<Document>>("Followers")[0].GetField<List<string>>("Interests").Count);
            Assert.AreEqual(person.Followers[0].Interests[0], document.GetField<List<Document>>("Followers")[0].GetField<List<string>>("Interests")[0]);
            Assert.AreEqual(person.Followers[0].Interests[1], document.GetField<List<Document>>("Followers")[0].GetField<List<string>>("Interests")[1]);
            Assert.AreEqual(person.Followers[1].FirstName, document.GetField<List<Document>>("Followers")[1].GetField<string>("FirstName"));
            Assert.AreEqual(person.Followers[1].LastName, document.GetField<List<Document>>("Followers")[1].GetField<string>("LastName"));
            Assert.AreEqual(person.Followers[1].Interests[0], document.GetField<List<Document>>("Followers")[1].GetField<List<string>>("Interests")[0]);
            Assert.AreEqual(person.Followers[1].Interests[1], document.GetField<List<Document>>("Followers")[1].GetField<List<string>>("Interests")[1]);
        }
        
        [Test()]
        public void Should_convert_to_generic_object_with_deserialized_only_field()
        {
            var document = new Document();
            document.SetField("FirstName", "Johny");
            document.SetField("LastName", "Bravo");
            document.SetField("ShouldBeOnlyDeserialized", "Dummy text");
            document.SetField("ShouldNotBeSerializedOrDeserialized", "Dummy text");
            
            var person = document.To<Person>();
            
            Assert.AreEqual(document.GetField<string>("FirstName"), person.FirstName);
            Assert.AreEqual(document.GetField<string>("LastName"), person.LastName);
            Assert.AreEqual(document.GetField<string>("ShouldBeOnlyDeserialized"), person.ShouldBeOnlyDeserialized);
            Assert.AreEqual(null, person.ShouldNotBeSerializedOrDeserialized);
        }
        
        [Test()]
        public void Should_evaluate_simple_documents_as_equal()
        {
            var document1 = new Document()
                .SetField("foo", "string value 1")
                .SetField("bar", 123);
            
            var document2 = new Document()
                .SetField("foo", "string value 1")
                .SetField("bar", 123);
            
            var areEqual = document1.Compare(document2);
            
            Assert.AreEqual(true, areEqual);
        }
        
        [Test()]
        public void Should_evaluate_simple_documents_as_not_equal()
        {
            var document1 = new Document()
                .SetField("foo", "string value 1")
                .SetField("bar", 111);
            
            var document2 = new Document()
                .SetField("foo", "string value 1")
                .SetField("bar", 123);
            
            var areEqual = document1.Compare(document2);
            
            Assert.AreEqual(false, areEqual);
        }
        
        [Test()]
        public void Should_evaluate_nested_documents_as_equal()
        {
            var document1 = new Document()
                .SetField("baz.foo", "string value 1")
                .SetField("baz.bar", 123);
            
            var document2 = new Document()
                .SetField("baz.foo", "string value 1")
                .SetField("baz.bar", 123);
            
            var areEqual = document1.Compare(document2);
            
            Assert.AreEqual(true, areEqual);
        }
        
        [Test()]
        public void Should_evaluate_nested_documents_as_not_equal()
        {
            var document1 = new Document()
                .SetField("baz.foo", "string value 1")
                .SetField("baz.bar", 123);
            
            var document2 = new Document()
                .SetField("baz.foo", "string value 1")
                .SetField("baz.bar", 111);
            
            var areEqual = document1.Compare(document2);
            
            Assert.AreEqual(false, areEqual);
        }
        
        [Test()]
        public void Should_evaluate_documents_with_nested_collection_as_equal()
        {
            var document1 = new Document()
                .SetField("baz.foo", new List<string> { "one", "two", "three" })
                .SetField("baz.bar", 123)
                .SetField<object>("baz.null", null);
            
            var document2 = new Document()
                .SetField("baz.foo", new List<string> { "one", "two", "three" })
                .SetField("baz.bar", 123)
                .SetField<object>("baz.null", null);
            
            var areEqual = document1.Compare(document2);
            
            Assert.AreEqual(true, areEqual);
        }
        
        [Test()]
        public void Should_evaluate_documents_with_nested_collection_as_not_equal()
        {
            var document1 = new Document()
                .SetField("baz.foo", new List<string> { "one", "two", "three" })
                .SetField("baz.bar", 123)
                .SetField<object>("baz.null", null);
            
            var document2 = new Document()
                .SetField("baz.foo", new List<string> { "one", "two", "four" })
                .SetField("baz.bar", 123)
                .SetField<object>("baz.null", null);
            
            var areEqual = document1.Compare(document2);
            
            Assert.AreEqual(false, areEqual);
        }
        
        [Test()]
        public void Should_evaluate_documents_with_null_value_as_not_equal()
        {
            var document1 = new Document()
                .SetField("baz.foo", new List<string> { "one", "two", "three" })
                .SetField("baz.bar", 123)
                .SetField<object>("baz.null", null);
            
            var document2 = new Document()
                .SetField("baz.foo", new List<string> { "one", "two", "three" })
                .SetField("baz.bar", 123)
                .SetField<object>("baz.null", "whoa");
            
            var areEqual = document1.Compare(document2);
            
            Assert.AreEqual(false, areEqual);
        }
    }
}
