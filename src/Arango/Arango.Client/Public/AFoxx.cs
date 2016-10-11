using System.Collections.Generic;
using Arango.Client.Protocol;
using Arango.fastJSON;

namespace Arango.Client
{
    public class AFoxx
    {
        readonly Dictionary<string, object> _parameters = new Dictionary<string, object>();
        readonly Connection _connection;

        internal AFoxx(Connection connection)
        {
            _connection = connection;
        }

        #region Parameters

        /// <summary>
        /// 
        /// </summary>
        public AFoxx Body(object value)
        {
            _parameters.Object(ParameterName.Body, value);

            return this;
        }

        #endregion

        /// <summary>
        /// 
        /// </summary>
        public AResult<T> Get<T>(string relativeUri)
        {
            var request = new Request(HttpMethod.GET, relativeUri);
            var response = _connection.Send(request);
            var result = new AResult<T>(response);

            result.Value = response.ParseBody<T>();

            _parameters.Clear();

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        public AResult<T> Post<T>(string relativeUri)
        {
            var request = new Request(HttpMethod.POST, relativeUri);

            if (_parameters.Has(ParameterName.Body))
            {
                request.Body = JSON.ToJSON(_parameters.Object(ParameterName.Body), ASettings.JsonParameters);
            }

            var response = _connection.Send(request);
            var result = new AResult<T>(response);

            result.Value = response.ParseBody<T>();

            _parameters.Clear();

            return result;
        }
    }
}
