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
        /// Determines whether collection should be created if it does not exist. Default value: false.
        /// </summary>
        public AGraph AddEdgeDefinition(string collection, List<string> fromList, List<string> toList)
        {
            var edgeDifinition = new Dictionary<string, object>
            {
                { "collection",  collection },
                { "from", fromList },
                { "to", toList }
            };

            _parameters.Object(ParameterName.EdgeDefinitions + "[*]", edgeDifinition);

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

                    result.Success = (body != null);
                    result.Value = body;
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

        #endregion


    }
}
