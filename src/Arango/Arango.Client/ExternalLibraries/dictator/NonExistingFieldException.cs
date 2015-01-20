using System;

namespace Arango.Client
{
	public class NonExistingFieldException : Exception
	{
		public NonExistingFieldException(string message) : base(message)
		{
		}
	}
}
