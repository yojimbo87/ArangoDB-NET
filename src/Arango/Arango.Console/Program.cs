using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using ServiceStack.Text;
using Arango.Client;

namespace Arango.Console
{
    class Program
    {
        static ArangoDatabase _database;

        static void Main(string[] args)
        {

            string alias = "test";
            string[] connectionString = File.ReadAllText(@"..\..\..\..\..\ConnectionString.txt").Split(';');

            ArangoNode node = new ArangoNode(
                connectionString[0],
                int.Parse(connectionString[1]),
                connectionString[2],
                connectionString[3],
                alias
            );
            ArangoClient.Nodes.Add(node);

            _database = new ArangoDatabase(alias);

            //ArangoCollection collection = database.GetCollection(10843274);
            //ArangoCollection collection = database.GetCollection("Users");
            //System.Console.WriteLine("ID: {0}, Name: {1}, Status: {2}, Type: {3}, WaitForSync: {4}, JournalSize: {5}", collection.ID, collection.Name, collection.Status, collection.Type, collection.WaitForSync, collection.JournalSize);

            /*ArangoDocument doc = new ArangoDocument();
            doc.Json.foo = "abc";
            doc.Json.bar = new ExpandoObject();
            doc.Json.bar.baz = 123;

            System.Console.WriteLine("foo: {0} {1}", doc.Has("foo"), doc.Json.foo);
            System.Console.WriteLine("bar: {0} {1}", doc.Has("bar"), doc.Json.bar);
            System.Console.WriteLine("bar.baz: {0} {1}", doc.Has("bar.baz"), doc.Json.bar.baz);
            System.Console.WriteLine("non: {0}", doc.Has("non"));
            System.Console.WriteLine("non.exist: {0}", doc.Has("non.exist"));*/

            //TestGet();
            //TestQuery();
            //TestText();
            TestDictionary();

            System.Console.ReadLine();
        }

        static void TestGet()
        {
            ArangoDocument document = _database.GetDocument("10843274/12481674");
            
            //System.Console.WriteLine("Handle: {0}, Rev: {1}, Json: {2}", document.ID, document.Revision, document.JsonObject.Stringify());
        }

        static void TestQuery()
        {
            Dictionary<string, string> values = new Dictionary<string, string>();
            values.Add("@title", "Test kiosk number 5");
            List<ArangoDocument> documents = _database.Query("FOR k IN Kiosks FILTER k.Title == @title RETURN k", false, 2, values);

            documents.ForEach(d => System.Console.WriteLine(d.ID));
        }

        static void TestText()
        {
            /*JsonObject json = "{\"foo\":{\"bar\":[{\"baz\":123},{\"baz\":456}]}}".ToJson();

            List<Json> bar = json.Get<JsonObject>("foo").Get<List<Json>>("bar");
            System.Console.WriteLine(bar[0].Get<int>("baz"));
            
            JsonObject baz = new JsonObject();
            baz.Add("baz", );
            bar.Add(
            
            System.Console.WriteLine(json.Stringify());*/
        }

        static void TestDictionary()
        {
            /*Foo json = new Foo();
            json.Add("stringKey", "string value");
            json.Add("numericKey", 12321);

            Foo embeddedJson = new Foo();
            embeddedJson.Add("stringKey", "string value");
            embeddedJson.Add("numericKey", 12321);

            json.Add("innerObjectKey", embeddedJson);

            List<Foo> arrayKey = new List<Foo>();
            arrayKey.Add(embeddedJson);
            arrayKey.Add(embeddedJson);

            json.Add("arrayKey", arrayKey);

            Foo embeddedJson2 = new Foo();
            embeddedJson2.Add("stringKey", "string value");
            embeddedJson2.Add("numericKey", 12321);
            embeddedJson2.Add("arrayKey", arrayKey);

            json.Add("embeddedKey", embeddedJson2);

            System.Console.WriteLine(json.ToJson());*/

            string s = "{\"stringKey\":\"string value\",\"numericKey\":12321,\"arrayEmptyKey\":[],\"arrayKey\":[1,2,3],\"arrayEmbeddedKey\":[{\"foo\":123},{\"foo\":456}],\"embeddedKey\":{\"numeric\":321,\"array\":[3,2,1]}}";
            string s2 = "{\"array\":[\"whoa\",123]}";
            string s3 = "[\"a\",\"b\"]";
            //Foo json = s2.FromJson<Foo>();

            Json foo = new Json();

            foo.Load(s);

            foo.PrintDump();
            System.Console.WriteLine(foo.Stringify());
        }
    }

    public class Foo : Dictionary<string, object>
    {
        //public int bar { get; set; }
    }
}
