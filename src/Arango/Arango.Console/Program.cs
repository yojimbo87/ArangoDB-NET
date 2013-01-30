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

            //database.GetCollections();
            //ArangoDocument document = database.GetDocument("10843274/12481674");
            //System.Console.WriteLine("Handle: {0}, Rev: {1}, Json: {2}", document.ID, document.Revision, document.JsonObject);

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

            TestQuery();

            System.Console.ReadLine();
        }

        static void TestQuery()
        {
            List<ArangoDocument> documents = _database.Query("FOR k IN Kiosks RETURN k");

            documents.ForEach(d => System.Console.WriteLine(d.ID));
        }
    }
}
