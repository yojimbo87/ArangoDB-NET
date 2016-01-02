using System;
using System.Collections.Generic;
using Arango.Client;
using Arango.fastJSON;

namespace Arango.ConsoleTests
{
    class Program
    {
        public static void Main(string[] args)
        {
            //InsertTest();
            //PerformanceTests();
            
            /*JsonToObjectHeatUp();
            JsonToObjectAsDictionaryHeatup();
            JsonParseHeatUp();
            
            for (int i = 0; i < 100; i++)
            {
                Console.WriteLine("Iteration: {0}", i + 1);
                
                JsonToObjectTest();
                JsonToObjectAsDictionaryTest();
                JsonParseTest();
            }*/
            
            Console.Write("Press any key to continue . . . ");
            Console.ReadKey(true);
        }
        
        static void InsertTest()
        {
            Database.CleanupTestDatabases();
            Database.CreateTestDatabase(Database.TestDatabaseGeneral);
            Database.CreateTestCollection("Release", ACollectionType.Document);
            
            var db = new ADatabase(Database.Alias);
            
            var album = new Release { Id = 123 , Status = "some status", Title = "some title", Country = "some country", Released = "released" };
            var releaseKey = CreateDocument(db, "Release", album);
        }
        
        static string CreateDocument(ADatabase db, string collection, object dataType)
        {
            var createDocumentResult = db.Document
                .WaitForSync(false)
                .Create(collection, dataType);
            
            var key = "";
            
            if (createDocumentResult.Success)
            {
                key = createDocumentResult.Value.String("_key");
            }
            
            return key;
        }
        
        static void PerformanceTests()
        {
            var performance = new Performance();
            
            //performance.TestSimpleSequentialHttpPostRequests();
            //performance.TestRestSharpHttpPostRequests();
            //performance.TestSimpleParallelHttpPostRequests();
            
            performance.Dispose();
        }
        
        static void JsonToObjectHeatUp()
        {
            var json = "{\"Foo\":\"some string\",\"Bar\":123}";
            
            //var obj = JSON.ToObject<Dictionary<string, object>>(json);
            var obj = JSON.ToObject<Dummy>(json);
            
            Console.WriteLine(obj);
        }
        
        static void JsonToObjectTest()
        {
            var json = "{\"Foo\":\"some string\",\"Bar\":123}";
            
            //var obj = JSON.ToObject<Dictionary<string, object>>(json);
            var obj = JSON.ToObject<Dummy>(json);
            
            Console.WriteLine(obj);
        }
        
        static void JsonToObjectAsDictionaryHeatup()
        {
            var json = "{\"Foo\":\"some string\",\"Bar\":123}";
            
            //var obj = JSON.ToObject<Dictionary<string, object>>(json);
            var obj = JSON.ToObject<Dictionary<string, object>>(json);
            
            Console.WriteLine(obj);
        }
        
        static void JsonToObjectAsDictionaryTest()
        {
            var json = "{\"Foo\":\"some string\",\"Bar\":123}";
            
            //var obj = JSON.ToObject<Dictionary<string, object>>(json);
            var obj = JSON.ToObject<Dictionary<string, object>>(json);
            
            Console.WriteLine(obj);
        }
        
        static void JsonParseHeatUp()
        {
            var json = "{\"Foo\":\"some string\",\"Bar\":123}";
            
            //var obj = JSON.Parse(json) as Dictionary<string, object>;
            var obj = GetObject<Dummy>(JSON.Parse(json) as Dictionary<string, object>);
            
            Console.WriteLine(obj);
        }
        
        static void JsonParseTest()
        {
            var json = "{\"Foo\":\"some string\",\"Bar\":123}";
            
            //var obj = JSON.Parse(json) as Dictionary<string, object>;
            var obj = GetObject<Dummy>(JSON.Parse(json) as Dictionary<string, object>);
            
            Console.WriteLine(obj);
        }
        
        static Object GetObject(Dictionary<string, object> dict, Type type)
        {
            var obj = Activator.CreateInstance(type);

            foreach (var kv in dict)
            {
                var prop = type.GetProperty(kv.Key);
                if(prop == null) continue;
    
                object value = kv.Value;
                if (value is Dictionary<string, object>)
                {
                    value = GetObject((Dictionary<string, object>) value, prop.PropertyType); // <= This line
                }
    
                prop.SetValue(obj, value, null);
            }
            
            return obj;
        }
        
        static T GetObject<T>(Dictionary<string, object> dict)
        {
            return (T)GetObject(dict, typeof(T));
        }
    }
    
    public class Dummy
    {
        public string Foo { get; set; }
        public long Bar { get; set; }
    }
}