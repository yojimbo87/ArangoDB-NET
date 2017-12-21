﻿using System.Collections.Generic;
using System.Text.RegularExpressions;
using Arango.fastJSON;
using Arango.Client.Protocol;

namespace Arango.Client
{
    /// <summary>
    /// Represents ArangoDB client specific settings and pool of connections.
    /// </summary>
    public static class ASettings
    {
        static readonly Dictionary<string, Connection> _connections = new Dictionary<string, Connection>();
        
        internal readonly static Regex KeyRegex = new Regex(@"^[a-zA-Z0-9_\-:.@()+,=;$!*'%]*$");
        
        /// <summary>
        /// Determines driver name.
        /// </summary>
        public const string DriverName = "ArangoDB-NET";
        
        /// <summary>
        /// Determines driver version.
        /// </summary>
        public const string DriverVersion = "0.11.0";

        /// <summary>
        /// Determines JSON serialization options. Default value is { UseEscapedUnicode = false, UseFastGuid = false, UseExtensions = false }.
        /// </summary>
        public static JSONParameters JsonParameters { get; set; }
        
        /// <summary>
        /// Determines whether HTTP requests which return status code indicating an error (e.g. 4xx or 5xx) should also throw exceptions and not only contain error data within result object. Default value is false.
        /// </summary>
        public static bool ThrowExceptions { get; set; }

        static ASettings()
        {
            JsonParameters = new JSONParameters { UseEscapedUnicode = false, UseFastGuid = false, UseExtensions = false };
            ThrowExceptions = false;
        }
        
        #region AddConnection
        
        public static void AddConnection(string alias, string endpoint, bool useWebProxy = false)
        {
            AddConnection(alias, endpoint, "", "", useWebProxy);
        }

        public static void AddConnection(string alias, string endpoint, string username, string password, bool useWebProxy = false)
        {
            var connection = new Connection(alias, endpoint, username, password, useWebProxy);

            _connections.Add(alias, connection);
        }
        
        public static void AddConnection(string alias, string endpoint, string databaseName, bool useWebProxy = false)
        {
            AddConnection(alias, endpoint, databaseName, "", "", useWebProxy);
        }
        
        public static void AddConnection(string alias, string endpoint, string databaseName, string username, string password, bool useWebProxy = false)
        {
            var connection = new Connection(alias, endpoint, databaseName, username, password, useWebProxy);

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