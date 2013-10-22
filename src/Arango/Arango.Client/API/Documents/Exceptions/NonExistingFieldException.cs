using System;

namespace Arango.Client
{
    /// <summary> 
    /// Represents error which happens when document field does not exist.
    /// </summary>
    public class NonExistingFieldException : Exception
    {
        public NonExistingFieldException(string message) : base(message)
        {
        }
    }
}
