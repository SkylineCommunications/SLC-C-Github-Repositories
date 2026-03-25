namespace Skyline.DataMiner.Scripting.Helper.Events
{
	using System;
	using System.Collections.Generic;
	using System.Linq;

	using Skyline.DataMiner.Scripting;

	public class DeleteRowEventArgs : TableEventArgs
	{
		public DeleteRowEventArgs(SLProtocol protocol, string primaryKey)
			: base(protocol)
		{
			PrimaryKeys = new[] { primaryKey } ?? throw new ArgumentNullException(nameof(primaryKey));
		}

		public DeleteRowEventArgs(SLProtocol protocol, ISet<string> primaryKeys)
			: base(protocol)
		{
			PrimaryKeys = primaryKeys.ToArray() ?? throw new ArgumentNullException(nameof(primaryKeys));
		}

		public string[] PrimaryKeys { get; }
	}
}
