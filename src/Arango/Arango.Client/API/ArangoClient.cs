using System.Collections.Generic;
using System.Linq;
using Arango.Client.Protocol;

namespace Arango.Client
{
    public static class ArangoClient
    {
        private static List<Connection> _connections = new List<Connection>();

        public static string DriverName
        {
            get { return "ArangoDB-NET"; }
        }

        public static string DriverVersion
        {
            get { return "0.5.0"; }
        }

        public static void AddDatabase(string hostname, int port, bool isSecured, string userName, string password, string alias)
        {
            var connection = new Connection(hostname, port, isSecured, userName, password, alias);

            _connections.Add(connection);
        }

        internal static Connection GetConnection(string alias)
        {
            return _connections.Where(connection => connection.Alias == alias).FirstOrDefault();
        }
    }
}

