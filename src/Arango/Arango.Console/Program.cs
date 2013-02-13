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
            TestQuery();
            //TestLoad();

            System.Console.ReadLine();
        }

        static void TestGet()
        {
            ArangoDocument document = _database.GetDocument("10843274/12481674");
            
            //System.Console.WriteLine("Handle: {0}, Rev: {1}, Json: {2}", document.ID, document.Revision, document.JsonObject.Stringify());
        }

        static void TestQuery()
        {
            Dictionary<string, object> values = new Dictionary<string, object>();
            values.Add("title", "Test kiosk number 5");
            List<ArangoDocument> documents = _database.Query("FOR k IN Kiosks FILTER k.Title == @title RETURN k", false, 2, values);

            //List<ArangoDocument> documents = _database.Query("FOR k IN Kiosks RETURN k");

            documents.ForEach(d => System.Console.WriteLine(d.ID));
        }

        static void TestLoad()
        {
            int repetitions = 20;
            long total = 0;

            for (int i = 0; i < repetitions; i++)
            {
                long tps = Do();
                total += tps;

                System.Console.WriteLine("TPS: " + tps);
            }

            System.Console.WriteLine("Average: " + total / repetitions);
        }

        static long Do()
        {
            DateTime start = DateTime.Now;
            bool running = true;
            long tps = 0;

            do
            {
                List<ArangoDocument> documents = _database.Query("FOR k IN Kiosks RETURN k");
                tps++;

                TimeSpan dif = DateTime.Now - start;

                if (dif.TotalMilliseconds > 1000)
                {
                    running = false;
                }
            }
            while (running);

            return tps;

            /*for (int i = 0; i < 1000; i++)
            {
                List<RexsterVertex> vertices = graph.Gremlin<RexsterVertex>("vx", new string[] { "vertices" });
            }

            //Parallel.For(0, 1000, i =>
            //{
            //    List<RexsterVertex> vertices = graph.Gremlin<RexsterVertex>("vx", new string[] { "vertices" });
            //});

            TimeSpan dif = DateTime.Now - start;

            System.Console.WriteLine("Time: " + dif.TotalSeconds);*/
        }
    }
}
