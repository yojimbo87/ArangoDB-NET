using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
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
            TestText();

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
            /*string json = "{\"_id\":123,\"foo\":{\"bar\":456}}";
            JsonObject obj = JsonObject.Parse(json);
            System.Console.WriteLine(obj.Dump());
            System.Console.WriteLine(obj.Get<JsonObject>("foo").Get("bar"));*/

            /*Dictionary<string, object> o = new Dictionary<string, object>();
            o.Add("foo", 123);

            Dictionary<string, string> b = new Dictionary<string, string>();
            b.Add("bar", "bbbb");
            o.Add("bar", b);*/

            Json json = new Json();
            /*json.Load("{\"_id\":123,\"foo\":{\"bar\":{\"_baz\":456},\"baz\":[\"a\",\"bbb\"]}}");

            System.Console.WriteLine(json.Get<int>("foo.bar._baz"));
            json.Set("foo.bar._baz", new List<string>() { "w", "a", "o" });
            System.Console.WriteLine(json.Get<List<string>>("foo.bar._baz")[0]);

            System.Console.WriteLine(json.Get<List<string>>("foo.baz")[1]);*/

            json.Load("{\"foo\":[{\"bar\":123},{\"bar\":456}]}");
            //json.Load("{\"foo\":[\"bar1\",\"bar2\"]}");
            List<Json> foo = json.GetValue<List<Json>>("foo");
            System.Console.WriteLine(foo[1].GetValue<int>("bar"));
            System.Console.WriteLine(json.Stringify());
        }
    }

    public class Foo
    {
        public int bar { get; set; }
    }
}
