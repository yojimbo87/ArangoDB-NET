using System;

namespace Arango.Client
{
	public class InvalidFieldException : Exception
	{
		public InvalidFieldException(string message) : base(message)
		{
		}
	}
}
