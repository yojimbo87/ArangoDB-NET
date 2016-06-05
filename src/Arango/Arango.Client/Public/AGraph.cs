using System;
using System.Collections.Generic;
using Arango.Client.Protocol;
using Arango.fastJSON;

namespace Arango.Client
{
    public class AGraph
    {
        readonly Connection _connection;

        internal AGraph(Connection connection)
        {
            _connection = connection;
        }

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
