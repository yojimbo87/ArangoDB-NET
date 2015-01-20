using System;

namespace Arango.Client
{
	public class InvalidFieldTypeException : Exception
	{
		public InvalidFieldTypeException(string message) : base(message)
		{
		}
	}
}
