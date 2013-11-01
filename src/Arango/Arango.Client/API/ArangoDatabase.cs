using System.Collections.Generic;
using Arango.Client.Protocol;

namespace Arango.Client
{
    public class ArangoDatabase
    {
        private Connection _connection;

        public ArangoCollectionOperation Collection
        {
            get
            {
                return new ArangoCollectionOperation(new CollectionOperation(_connection));
            }
        }
        
        public ArangoDocumentOperation Document
        {
            get
            {
                return new ArangoDocumentOperation(new DocumentOperation(_connection));
            }
        }
        
        public ArangoEdgeOperation Edge
        {
            get
            {
                return new ArangoEdgeOperation(new EdgeOperation(_connection));
            }
        }
        
        public ArangoQueryOperation Query
        {
            get
            {
                return new ArangoQueryOperation(new CursorOperation(_connection));
            }
        }
        
        public ArangoFunctionOperation Function
        {
            get
            {
                return new ArangoFunctionOperation(new FunctionOperation(_connection));
            }
        }

        public ArangoDatabase(string alias)
        {
            _connection = ArangoClient.GetConnection(alias);
        }
    }
}

