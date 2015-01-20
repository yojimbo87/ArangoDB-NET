using System;
using System.Net;
using Arango.fastJSON;

namespace Arango.Client.Protocol
{
    internal class Response
    {
        internal int StatusCode { get; set; }
        internal WebHeaderCollection Headers { get; set; }
        internal string Body { get; set; }
        internal DataType DataType { get; set; }
        internal object Data { get; set; }
        internal Exception Exception { get; set; }
        internal AEerror Error { get; set; }
        
        internal void DeserializeBody()
        {            
            if (string.IsNullOrEmpty(Body))
            {
                DataType = DataType.Null;
                Data = null;
            }
            else
            {
                var trimmedBody = Body.Trim();
                
                // body contains JSON array
                if (trimmedBody[0] == '[')
                {
                    DataType = DataType.List;
                }
                // body contains JSON object
                else if (trimmedBody[0] == '{')
                {
                    DataType = DataType.Document;
                }
                else
                {
                    DataType = DataType.Primitive;
                }
                
                Data = JSON.Parse(trimmedBody);
            }
        }
    }
}
