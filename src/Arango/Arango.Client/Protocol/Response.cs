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

                switch (trimmedBody[0])
                {
                    // body contains JSON array
                    case '[':
                        DataType = DataType.List;
                        break;
                    // body contains JSON object
                    case '{':
                        DataType = DataType.Document;
                        break;
                    default:
                        DataType = DataType.Primitive;
                        break;
                }
                
                Data = JSON.Parse(trimmedBody);
            }
        }
    }
}
