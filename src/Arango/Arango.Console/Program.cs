using System;
using System.Collections.Generic;
using Arango.Client;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Arango.Console
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            ArangoClient.AddConnection(
                "localhost",
                8529,
                false,
                "test",
                "test"
            );

            var db = new ArangoDatabase("test");

            var document = db.Document.Get("Usersasfd/21857976");

            System.Console.WriteLine(document.String("_id"));
            System.Console.WriteLine(document.String("_key"));
            System.Console.WriteLine(document.String("_rev"));
            
            /*ArangoCollection collection = db.Collection.Get("Users");

            System.Console.WriteLine(collection.Id);
            System.Console.WriteLine(collection.Name);
            System.Console.WriteLine(collection.Status);
            System.Console.WriteLine(collection.Type);*/
            
            /*ArangoCollection col1 = new ArangoCollection();
            col1.Name = "latif";
            col1.Type = ArangoCollectionType.Edge;
            col1.WaitForSync = true;
            col1.JournalSize = 64000;
            
            db.Collection.Create(col1);
            
            System.Console.WriteLine(col1.Id);
            System.Console.WriteLine(col1.Name);
            System.Console.WriteLine(col1.Status);
            System.Console.WriteLine(col1.Type);
            
            System.Console.WriteLine(db.Collection.Delete(col1.Name));*/
            
            /*string json = @"{
                ""integer"": 12345,
                ""_foo"": ""bar"",
                ""embedded"": {
                    ""bar"": ""baz""
                },
                ""array"": [1, 2, 3, 4, 5],
                ""complex"": [{""x"":111,""y"":222},{""y"":333}]
            }";*/

            /*var json = "{\"null\":null,\"embedded\":{\"null\":null}}";

            var doc = new ArangoDocument(json);

            System.Console.WriteLine(JsonConvert.SerializeObject(doc, Formatting.Indented));

            /*Dictionary<string, object> foo = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);

            foreach (KeyValuePair<string, object> item in foo)
            {
                System.Console.WriteLine("{0} {1} {2}", item.Key, item.Value, item.Value.GetType());
            }

            System.Console.WriteLine("{0} {1}", ((JObject)foo["embedded"])["bar"], ((JObject)foo["embedded"]).Type);*/

            /*Dictionary<string, object> bar = new Dictionary<string, object>();

            bar.Add("foo", "bar");
            bar.Add("int", 123);
            bar.Add("emb", foo);
            bar.Add("arr", new string[] {"a", "b", "c"});

            System.Console.WriteLine(JsonConvert.SerializeObject(bar, Formatting.Indented));*/

            System.Console.ReadLine();
        }
    }
}

