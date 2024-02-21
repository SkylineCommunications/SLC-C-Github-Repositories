// Ignore Spelling: IAC

namespace Skyline.Protocol.Tables
{
	using System;

	using Skyline.DataMiner.Scripting;

	public delegate void IAC_MessagesTableEventHandler(object sender, IAC_MessagesTableRow e);

	public enum IAC_MessagesChange
	{
		Add,
		Remove,
	}

	public class IAC_MessagesEventArgs : EventArgs
	{
		public IAC_MessagesEventArgs(SLProtocol protocol, IAC_MessagesChange changeType, params string[] repositories)
		{
			Protocol = protocol;
			Type = changeType;
			Repositories = repositories;
		}

		public SLProtocol Protocol { get; }

		public IAC_MessagesChange Type { get; }

		public string[] Repositories { get; }
	}
}
