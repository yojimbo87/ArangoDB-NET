using System.Collections.Generic;
using Arango.Client.Protocol;

namespace Arango.Client
{
    /// <summary> 
    /// Provides functionality to manage specific Arango database.
    /// </summary>
    public class ArangoDatabase
    {
        private Connection _connection;

        /// <summary> 
        /// Expose collection management functionality.
        /// </summary>
        public ArangoCollectionOperation Collection
        {
            get
            {
                return new ArangoCollectionOperation(new CollectionOperation(_connection));
            }
        }
        
        /// <summary> 
        /// Expose document management functionality.
        /// </summary>
        public ArangoDocumentOperation Document
        {
            get
            {
                return new ArangoDocumentOperation(new DocumentOperation(_connection));
            }
        }
        
        /// <summary> 
        /// Expose edge management functionality.
        /// </summary>
        public ArangoEdgeOperation Edge
        {
            get
            {
                return new ArangoEdgeOperation(new EdgeOperation(_connection));
            }
        }
        
        /// <summary> 
        /// Expose AQL querying functionality.
        /// </summary>
        public ArangoQueryOperation Query
        {
            get
            {
                return new ArangoQueryOperation(new CursorOperation(_connection));
            }
        }
        
        /// <summary> 
        /// Expose AQL function management functionality.
        /// </summary>
        public ArangoFunctionOperation Function
        {
            get
            {
                return new ArangoFunctionOperation(new FunctionOperation(_connection));
            }
        }

        /// <summary> 
        /// Expose server functionality
        /// </summary>
        public ArangoServerOperation Server
        {
            get
            {
                return new ArangoServerOperation(new ServerOperation(_connection));
            }
        }               
                
        /// <summary> 
        /// Expose version functionality
        /// </summary>
        public ArangoVersionOperation Version
        {
            get
            {
                return new ArangoVersionOperation(new VersionOperation(_connection));
            }
        }        

        /// <summary>
        /// Creates Arango database object with specified alias connection.
        /// </summary>
        /// <param name="alias">Connection alias which was previously created through ArangoClient.AddConnection method.</param>
        public ArangoDatabase(string alias)
        {
            _connection = ArangoClient.GetConnection(alias);
        }
    }
}

