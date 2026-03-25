namespace Skyline.DataMiner.Scripting.Helper.Exceptions
{
	using System;

	public class PrimaryKeyAlreadyExistsException : Exception
	{
		public PrimaryKeyAlreadyExistsException(int tablePid, string primaryKey)
			: base($"Table<{tablePid}>: Primary key with value '{primaryKey}' already exists.")
		{
		}
	}
}