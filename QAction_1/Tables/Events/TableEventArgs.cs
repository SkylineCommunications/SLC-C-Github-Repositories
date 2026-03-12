namespace Skyline.Protocol.Tables.Events
{
	using System;

	using Skyline.DataMiner.Scripting;

	public enum TableChange
	{
		Add,
		Remove,
	}

	public class TableEventArgs : EventArgs
	{
		public TableEventArgs(SLProtocol protocol, TableChange changeType, params string[] keys)
		{
			Protocol = protocol;
			Type = changeType;
			PrimaryKeys = keys;
		}

		public SLProtocol Protocol { get; }

		public TableChange Type { get; }

		public string[] PrimaryKeys { get; }
	}
}
