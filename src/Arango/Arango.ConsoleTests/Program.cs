using System;
using System.Collections.Generic;
using Arango.Client;
using Arango.fastJSON;

namespace Arango.ConsoleTests
{
    class Program
    {
        const string _alias = "test";
        
        public static void Main(string[] args)
        {
            ArangoSettings.AddConnection(
                _alias,
                "localhost",
                8529,
                false,
                "_system"
            );
            
            //GetCurrentDatabase();
            //GetAccessibleDatabases();
            //GetAllDatabases();
            //CreateDatabase();
            //DropDatabase();
            
            //CreateDocument();
            
            Console.Write("Press any key to continue . . . ");
            Console.ReadKey(true);
        }
        
        static void GetCurrentDatabase()
        {
            var db = new ArangoDatabase(_alias);
            
            var result = db.GetCurrent();

            if (!result.Success)
            {
                Console.WriteLine(result.Error.Message);
                
                return;
            }
            
            Console.WriteLine(JSON.ToNiceJSON(result.Value, new JSONParameters()));
        }
        
        static void GetAccessibleDatabases()
        {
            var db = new ArangoDatabase(_alias);
            
            var result = db.GetAccessibleDatabases();

            if (!result.Success)
            {
                Console.WriteLine(result.Error.Message);
                
                return;
            }
            
            Console.WriteLine(JSON.ToNiceJSON(result.Value, new JSONParameters()));
        }
        
        static void GetAllDatabases()
        {
            var db = new ArangoDatabase(_alias);
            
            var result = db.GetAllDatabases();

            if (!result.Success)
            {
                Console.WriteLine(result.Error.Message);
                
                return;
            }
            
            Console.WriteLine(JSON.ToNiceJSON(result.Value, new JSONParameters()));
        }
        
        static void CreateDatabase()
        {
            var db = new ArangoDatabase(_alias);
            
            var result = db.Create("whoa");

            if (!result.Success)
            {
                Console.WriteLine(result.Error.Message);
                
                return;
            }
            
            Console.WriteLine(JSON.ToNiceJSON(result.Value, new JSONParameters()));
        }
        
        static void DropDatabase()
        {
            var db = new ArangoDatabase(_alias);
            
            var result = db.Drop("whoa");

            if (!result.Success)
            {
                Console.WriteLine(result.Error.Message);
                
                return;
            }
            
            Console.WriteLine(JSON.ToNiceJSON(result.Value, new JSONParameters()));
        }
        
        static void CreateDocument()
        {
            var db = new ArangoDatabase(_alias);
            
            var doc = new Dictionary<string, object>()
                .String("foo", "bar");
            
            var result = db.Document.Create("TestCollection", doc);

            if (!result.Success)
            {
                Console.WriteLine(result.Error.Message);
                
                return;
            }
            
            Console.WriteLine(JSON.ToNiceJSON(result.Value, new JSONParameters()));
        }
    }
}