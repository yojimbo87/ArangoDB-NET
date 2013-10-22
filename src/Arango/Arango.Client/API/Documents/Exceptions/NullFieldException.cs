using System;

namespace Arango.Client
{
    /// <summary> 
    /// Represents error which happens when document field has null value.
    /// </summary>
    public class NullFieldException : Exception
    {
        public NullFieldException(string message) : base(message)
        {
        }
    }
}
