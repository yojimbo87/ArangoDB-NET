using System.Collections.Generic;
using System.Linq;
using Arango.Client.Protocol;

namespace Arango.Client
{
    public static class ArangoClient
    {
        private static Dictionary<string, Connection> _connections = new Dictionary<string, Connection>();

        public const string DriverName = "ArangoDB-NET";

        public const string DriverVersion = "0.7.0";
        
        public static ArangoSettings Settings { get; set; }
        
        static ArangoClient()
        {
            Settings = new ArangoSettings();
            Document.Settings.DateTimeFormat = DateTimeFormat.UnixTimeStamp;
        }

        public static void AddDatabase(string hostname, int port, bool isSecured, string userName, string password, string alias)
        {
            _connections.Add(
                alias,
                new Connection(hostname, port, isSecured, userName, password, alias)
            );
        }

        internal static Connection GetConnection(string alias)
        {
            return _connections[alias];
        }
    }
}

