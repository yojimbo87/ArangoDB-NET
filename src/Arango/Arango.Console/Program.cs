using System;
using System.IO;
using Arango.Client;

namespace Arango.Console
{
    class Program
    {
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

            ArangoDatabase database = new ArangoDatabase(alias);
            string s = database.Document("10843274/12481674");

            System.Console.WriteLine(s);

            System.Console.ReadLine();
        }
    }
}
