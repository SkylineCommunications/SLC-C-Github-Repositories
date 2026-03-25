namespace Skyline.DataMiner.Scripting.Helper.Exceptions
{
	using System;

	public class PrimaryKeyNotFoundException : Exception
	{
		public PrimaryKeyNotFoundException(int tablePid, string primaryKey)
			: base($"Table<{tablePid}>: Primary key with value '{primaryKey}' does not exist.")
		{
		}
	}
}