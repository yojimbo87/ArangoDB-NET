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
            ArangoDocument document = database.GetDocument("10843274/12481674", "x12481674");

            System.Console.WriteLine("ID: {0}, Rev: {1}, Data: {2}", document.ID, document.Revision, document.Data);

            System.Console.ReadLine();
        }
    }
}
