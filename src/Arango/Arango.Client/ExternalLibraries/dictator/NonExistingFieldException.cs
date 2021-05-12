using System;

namespace Arango.Client.ExternalLibraries.dictator
{
	public class NonExistingFieldException : Exception
	{
		public NonExistingFieldException(string message) : base(message)
		{
		}
	}
}
