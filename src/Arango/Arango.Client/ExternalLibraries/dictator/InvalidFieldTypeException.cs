using System;

namespace Arango.Client.ExternalLibraries.dictator
{
	public class InvalidFieldTypeException : Exception
	{
		public InvalidFieldTypeException(string message) : base(message)
		{
		}
	}
}
