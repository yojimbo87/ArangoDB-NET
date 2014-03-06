using System.Collections.Generic;
using System.Linq;
using Arango.Client.Protocol;

namespace Arango.Client
{
    public static class ArangoClient
    {
        private static Dictionary<string, Connection> _connections = new Dictionary<string, Connection>();

        /// <summary>
        /// Name of the driver.
        /// </summary>
        public const string DriverName = "ArangoDB-NET";

        /// <summary>
        /// Version of the driver.
        /// </summary>
        public const string DriverVersion = "0.8.1";
        
        /// <summary>
        /// Driver global settings object.
        /// </summary>
        public static ArangoSettings Settings { get; set; }
        
        static ArangoClient()
        {
            Settings = new ArangoSettings();
            Document.Settings.DateTimeFormat = DateTimeFormat.UnixTimeStamp;
        }
        
        /// <summary>
        /// Retrieves list of all existing databases. 
        /// </summary>
        /// <param name="hostname">Hostname of database connection.</param>
        /// <param name="port">Port number of database connection.</param>
        /// <param name="isSecured">Flag indicating if the database connection is HTTP or HTTPS.</param>
        /// <param name="userName">User name for authentication.</param>
        /// <param name="password">Password for authentication.</param>
        public static List<string> ListDatabases(string hostname, int port, bool isSecured, string userName = "", string password = "")
        {
            var databaseOperation = new DatabaseOperation(
                new Connection(hostname, port, isSecured, userName, password)
            );
            
            return databaseOperation.Get();
        }
        
        /// <summary>
        /// Creates new database with specified parameters. 
        /// </summary>
        /// <param name="hostname">Hostname of database connection.</param>
        /// <param name="port">Port number of database connection.</param>
        /// <param name="isSecured">Flag indicating if the database connection is HTTP or HTTPS.</param>
        /// <param name="databaseName">Name of the database to be created.</param>
        /// <param name="userName">User name for authentication.</param>
        /// <param name="password">Password for authentication.</param>
        public static bool CreateDatabase(string hostname, int port, bool isSecured, string databaseName, string userName = "", string password = "")
        {
            var databaseOperation = new DatabaseOperation(
                new Connection(hostname, port, isSecured, userName, password)
            );
            
            return databaseOperation.Post(databaseName);
        }
        
        /// <summary>
        /// Deletes specified database. 
        /// </summary>
        /// <param name="hostname">Hostname of database connection.</param>
        /// <param name="port">Port number of database connection.</param>
        /// <param name="isSecured">Flag indicating if the database connection is HTTP or HTTPS.</param>
        /// <param name="databaseName">Name of the database to be deleted.</param>
        /// <param name="userName">User name for authentication.</param>
        /// <param name="password">Password for authentication.</param>
        public static bool DeleteDatabase(string hostname, int port, bool isSecured, string databaseName, string userName = "", string password = "")
        {
            var databaseOperation = new DatabaseOperation(
                new Connection(hostname, port, isSecured, userName, password)
            );
            
            return databaseOperation.Delete(databaseName);
        }

        /// <summary>
        /// Stores connection data to existing database under specified alias which will be used by ArangoDatabase objects. 
        /// </summary>
        /// <param name="hostname">Hostname of database connection.</param>
        /// <param name="port">Port number of database connection.</param>
        /// <param name="isSecured">Flag indicating if the database connection is HTTP or HTTPS.</param>
        /// <param name="databaseName">Name of the database to which will be connection made.</param>
        /// <param name="alias">Alias under which will be database connection stored.</param>
        /// <param name="userName">User name for authentication.</param>
        /// <param name="password">Password for authentication.</param>
        public static void AddConnection(string hostname, int port, bool isSecured, string databaseName, string alias, string userName = "", string password = "")
        {
            _connections.Add(
                alias,
                new Connection(hostname, port, isSecured, databaseName, alias, userName, password)
            );
        }

        internal static Connection GetConnection(string alias)
        {
            return _connections[alias];
        }
    }
}

