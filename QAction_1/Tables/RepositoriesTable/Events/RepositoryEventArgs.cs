namespace Skyline.Protocol.Tables
{
	using System;

	using Skyline.DataMiner.Scripting;

	public delegate void RepositoryTableEventHandler(object sender, RepositoriesTableRow e);

	public enum RepositoryChange
	{
		Add,
		Remove,
	}

	public class RepositoryEventArgs : EventArgs
	{
		public RepositoryEventArgs(SLProtocol protocol, RepositoryChange changeType, params string[] repositories)
		{
			Protocol = protocol;
			Type = changeType;
			Repositories = repositories;
		}

		public SLProtocol Protocol { get; }

		public RepositoryChange Type { get; }

		public string[] Repositories { get; }
	}
}
