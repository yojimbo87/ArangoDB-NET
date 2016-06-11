using System;
using System.Collections.Generic;
using Arango.Client.Protocol;
using Arango.fastJSON;

namespace Arango.Client
{
    public class AGraph
    {
        readonly Dictionary<string, object> _parameters = new Dictionary<string, object>();
        readonly Connection _connection;

        internal AGraph(Connection connection)
        {
            _connection = connection;
        }

        #region Parameters

        /// <summary>
        /// Adds edge definition which represents directed relation of a graph.
        /// </summary>
        public AGraph AddEdgeDefinition(string collectionName, List<string> fromList, List<string> toList)
        {
            var edgeDifinition = new Dictionary<string, object>
            {
                { "collection",  collectionName },
                { "from", fromList },
                { "to", toList }
            };

            _parameters.Object(ParameterName.EdgeDefinitions + "[*]", edgeDifinition);

            return this;
        }

        /// <summary>
        /// Determines if delete operation should also drop collections of specified graph. Collections will only be dropped if they are not used in other graphs. Default value: false.
        /// </summary>
        public AGraph DropCollections(bool value)
        {
            _parameters.Bool(ParameterName.DropCollections, value);

            return this;
        }

        #endregion

        #region Create (POST)

        /// <summary>
        /// Creates new graph in current database context.
        /// </summary>
        public AResult<Dictionary<string, object>> Create(string graphName)
        {
            var request = new Request(HttpMethod.POST, ApiBaseUri.Gharial);
            var bodyDocument = new Dictionary<string, object>();

            // required
            bodyDocument.String(ParameterName.Name, graphName);
            // optional
            Request.TrySetBodyParameter(ParameterName.EdgeDefinitions, _parameters, bodyDocument);
            // TODO: orphan collection optional parameter

            request.Body = JSON.ToJSON(bodyDocument, ASettings.JsonParameters);

            var response = _connection.Send(request);
            var result = new AResult<Dictionary<string, object>>(response);

            switch (response.StatusCode)
            {
                case 201:
                    var body = response.ParseBody<Dictionary<string, object>>();

                    result.Success = (body != null) && body.Has("graph");

                    if (body.Has("graph"))
                    {
                        result.Value = body.Object<Dictionary<string, object>>("graph");
                    }
                    break;
                case 409:
                default:
                    // Arango error
                    break;
            }

            _parameters.Clear();

            return result;
        }

        #endregion

        #region Get (GET)

        /// <summary>
        /// Retrieves list of all graphs.
        /// </summary>
        public AResult<List<Dictionary<string, object>>> GetAllGraphs()
        {
            var request = new Request(HttpMethod.GET, ApiBaseUri.Gharial);

            var response = _connection.Send(request);
            var result = new AResult<List<Dictionary<string, object>>>(response);

            switch (response.StatusCode)
            {
                case 200:
                    var body = response.ParseBody<Dictionary<string, object>>();

                    result.Success = (body != null);
                    result.Value = body.List<Dictionary<string, object>>("graphs");
                    break;
                default:
                    // Arango error
                    break;
            }

            return result;
        }

        /// <summary>
        /// Retrieves specified graph.
        /// </summary>
        public AResult<Dictionary<string, object>> Get(string graphName)
        {
            var request = new Request(HttpMethod.GET, ApiBaseUri.Gharial, "/" + graphName);

            var response = _connection.Send(request);
            var result = new AResult<Dictionary<string, object>>(response);

            switch (response.StatusCode)
            {
                case 200:
                    var body = response.ParseBody<Dictionary<string, object>>();

                    result.Success = (body != null);
                    result.Value = body.Object<Dictionary<string, object>>("graph");
                    break;
                case 404:
                default:
                    // Arango error
                    break;
            }

            return result;
        }

        #endregion

        #region Delete (DELETE)

        /// <summary>
        /// Deletes specified graph and returns boolean value which indicates if operation was successful.
        /// </summary>
        public AResult<bool> Delete(string graphName)
        {
            var request = new Request(HttpMethod.DELETE, ApiBaseUri.Gharial, "/" + graphName);

            var response = _connection.Send(request);

            // optional
            request.TrySetQueryStringParameter(ParameterName.DropCollections, _parameters);

            var result = new AResult<bool>(response);

            switch (response.StatusCode)
            {
                case 200:
                    var body = response.ParseBody<Dictionary<string, object>>();

                    result.Success = (body != null);
                    result.Value = body.Bool("removed");
                    break;
                case 404:
                default:
                    // Arango error
                    break;
            }

            _parameters.Clear();

            return result;
        }

        #endregion
    }
}
