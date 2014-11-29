using System.Collections.Generic;
using Arango.Client.Protocol;

namespace Arango.Client
{
    public static class ArangoSettings
    {
        static readonly Dictionary<string, Connection> _connections = new Dictionary<string, Connection>();
        
        /// <summary>
        /// Driver name.
        /// </summary>
        public const string DriverName = "ArangoDB-NET";
        
        /// <summary>
        /// Driver version.
        /// </summary>
        public const string DriverVersion = "2.0.0";
        
        //public static ArangoGlobals Globals { get; private set; }
        
        static ArangoSettings()
        {
        	//Globals = new ArangoGlobals();
        }
        
        #region AddConnection
        
        public static void AddConnection(string alias, string hostname, int port, bool isSecured)
        {
            AddConnection(alias, hostname, port, isSecured, "", "");
        }

        public static void AddConnection(string alias, string hostname, int port, bool isSecured, string username, string password)
        {
            var connection = new Connection(alias, hostname, port, isSecured, username, password);

            _connections.Add(alias, connection);
        }
        
        public static void AddConnection(string alias, string hostname, int port, bool isSecured, string databaseName)
        {
            AddConnection(alias, hostname, port, isSecured, databaseName, "", "");
        }
        
        public static void AddConnection(string alias, string hostname, int port, bool isSecured, string databaseName, string username, string password)
        {
            var connection = new Connection(alias, hostname, port, isSecured, databaseName, username, password);

            _connections.Add(alias, connection);
        }
        
        #endregion

        public static void RemoveConnection(string alias)
        {
            if (_connections.ContainsKey(alias))
            {
                _connections.Remove(alias);
            }
        }
        
        public static bool HasConnection(string alias)
        {
        	return _connections.ContainsKey(alias);
        }

        internal static Connection GetConnection(string alias)
        {
            if (_connections.ContainsKey(alias))
            {
                return _connections[alias];
            }
                
            return null;
        }
    }
}