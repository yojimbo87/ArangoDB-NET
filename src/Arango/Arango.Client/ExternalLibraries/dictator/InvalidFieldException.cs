using System;

namespace Arango.Client.ExternalLibraries.dictator
{
	public class InvalidFieldException : Exception
	{
		public InvalidFieldException(string message) : base(message)
		{
		}
	}
}
