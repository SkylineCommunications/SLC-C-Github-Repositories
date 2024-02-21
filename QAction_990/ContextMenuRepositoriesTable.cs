namespace QAction_990
{
	using System;

	using Newtonsoft.Json;

	using Skyline.DataMiner.ConnectorAPI.Github.Repositories.InterAppMessages.Repositories;
	using Skyline.DataMiner.ConnectorAPI.Github.Repositories;
	using Skyline.DataMiner.Scripting;
	using Skyline.DataMiner.Utils.Table.ContextMenu;
	using Skyline.Protocol.InterApp;

	internal enum Action
	{
		Add = 1,
		Deleteselectedrow_40_s_41_ = 2,
	}

	internal class ContextMenuRepositoriesTable : ContextMenu<Action>
	{
		public ContextMenuRepositoriesTable(SLProtocol protocol, object contextMenuData, int tablePid)
			: base(protocol, contextMenuData, tablePid)
		{
		}

		public override void ProcessContextMenuAction()
		{
			switch (Action)
			{
				case Action.Add:
					Add();
					break;

				case Action.Deleteselectedrow_40_s_41_:
					Remove();
					break;

				default:
					Protocol.Log("QA" + Protocol.QActionID + "|ContextMenuRepositoriesTable|Process|Unexpected ContextMenu value '" + ActionRaw + "'", LogType.Error, LogLevel.NoLogging);
					break;
			}
		}

		protected void Add()
		{
			// Add through name and owner
			var request = new AddRepositoryRequest
			{
				RepositoryId = new RepositoryId(Data[1], Data[0]),
			};

			request.TryExecute(Protocol, Protocol, Mapping.MessageToExecutorMapping, out _);
		}

		protected void Remove()
		{
			foreach(var row in Data)
			{
				var owner = row.Split('/')[0];
				var name = row.Split('/')[1];
				var request = new RemoveRepositoryRequest
				{
					RepositoryId = new RepositoryId(owner, name),
				};

				request.TryExecute(Protocol, Protocol, Mapping.MessageToExecutorMapping, out _);
			}
		}
	}
}