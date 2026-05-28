namespace Skyline.Protocol.Tables
{
	using System;
	using System.Collections.Generic;
	using System.Linq;

	using Skyline.DataMiner.Scripting;
	using Skyline.DataMiner.Scripting.Helper;
	using Skyline.Protocol.PollManager;
	using Skyline.Protocol.Tables.Poll_Manager;

	public enum RepositoryType
	{
		Other = 0,
		Automation = 1,
		Connector = 2,
	}

	public class RepositoriesRowConverter : ISLRowConverter<RepositoriesModel, RepositoriesQActionRow>
	{
		public static readonly RepositoriesRowConverter Instance = new RepositoriesRowConverter();

		private readonly ColumnMapBase<RepositoriesModel>[] _columnMaps;
		private readonly ColumnWriteMapBase<RepositoriesModel>[] _columnWriteMaps;

		protected RepositoriesRowConverter()
		{
			_columnMaps = new ColumnMapBase<RepositoriesModel>[]
			{
				SLTables.Repositories.FullName.Read.Map<RepositoriesModel>(m => m.FullName),
				SLTables.Repositories.Name.Read.Map<RepositoriesModel>(m => m.Name),
				SLTables.Repositories.Private.Read.Map<RepositoriesModel>(m => m.Private),
				SLTables.Repositories.Description.Read.Map<RepositoriesModel>(m => m.Description),
				SLTables.Repositories.Owner.Read.Map<RepositoriesModel>(m => m.Owner),
				SLTables.Repositories.Fork.Read.Map<RepositoriesModel>(m => m.Fork),
				SLTables.Repositories.CreatedAt.Read.Map<RepositoriesModel>(m => m.CreatedAt),
				SLTables.Repositories.UpdatedAt.Read.Map<RepositoriesModel>(m => m.UpdatedAt),
				SLTables.Repositories.PushedAt.Read.Map<RepositoriesModel>(m => m.PushedAt),
				SLTables.Repositories.Size.Read.Map<RepositoriesModel>(m => m.Size),
				SLTables.Repositories.Stars.Read.Map<RepositoriesModel>(m => m.Stars),
				SLTables.Repositories.Watcher.Read.Map<RepositoriesModel>(m => m.Watcher),
				SLTables.Repositories.Language.Read.Map<RepositoriesModel>(m => m.Language),
				SLTables.Repositories.DefaultBranch.Read.Map<RepositoriesModel>(m => m.DefaultBranch),
				SLTables.Repositories.Type.Read.Map<RepositoriesModel>(m => m.Type),
				SLTables.Repositories.PublicKeyID.Read.Map<RepositoriesModel>(m => m.PublicKeyID),
				SLTables.Repositories.PublicKey.Read.Map<RepositoriesModel>(m => m.PublicKey),
				SLTables.Repositories.Id.Read.Map<RepositoriesModel>(m => m.Id),
				SLTables.Repositories.Topics.Read.Map<RepositoriesModel>(m => m.Topics),
				SLTables.Repositories.AutoRemove.Read.Map<RepositoriesModel>(m => m.AutoRemove),
			};
			_columnWriteMaps = new ColumnWriteMapBase<RepositoriesModel>[]
			{
				SLTables.Repositories.FullName.Read.MapWrite<RepositoriesModel>(m => m.FullName),
				SLTables.Repositories.Name.Read.MapWrite<RepositoriesModel>(m => m.Name),
				SLTables.Repositories.Private.Read.MapWrite<RepositoriesModel>(m => m.Private),
				SLTables.Repositories.Description.Read.MapWrite<RepositoriesModel>(m => m.Description),
				SLTables.Repositories.Owner.Read.MapWrite<RepositoriesModel>(m => m.Owner),
				SLTables.Repositories.Fork.Read.MapWrite<RepositoriesModel>(m => m.Fork),
				SLTables.Repositories.CreatedAt.Read.MapWrite<RepositoriesModel>(m => m.CreatedAt),
				SLTables.Repositories.UpdatedAt.Read.MapWrite<RepositoriesModel>(m => m.UpdatedAt),
				SLTables.Repositories.PushedAt.Read.MapWrite<RepositoriesModel>(m => m.PushedAt),
				SLTables.Repositories.Size.Read.MapWrite<RepositoriesModel>(m => m.Size),
				SLTables.Repositories.Stars.Read.MapWrite<RepositoriesModel>(m => m.Stars),
				SLTables.Repositories.Watcher.Read.MapWrite<RepositoriesModel>(m => m.Watcher),
				SLTables.Repositories.Language.Read.MapWrite<RepositoriesModel>(m => m.Language),
				SLTables.Repositories.DefaultBranch.Read.MapWrite<RepositoriesModel>(m => m.DefaultBranch),
				SLTables.Repositories.Type.Read.MapWrite<RepositoriesModel>(m => m.Type),
				SLTables.Repositories.PublicKeyID.Read.MapWrite<RepositoriesModel>(m => m.PublicKeyID),
				SLTables.Repositories.PublicKey.Read.MapWrite<RepositoriesModel>(m => m.PublicKey),
				SLTables.Repositories.Id.Read.MapWrite<RepositoriesModel>(m => m.Id),
				SLTables.Repositories.Topics.Read.MapWrite<RepositoriesModel>(m => m.Topics),
				SLTables.Repositories.AutoRemove.Read.MapWrite<RepositoriesModel>(m => m.AutoRemove),
			};
		}

		public RepositoriesModel FromRawValue(RepositoriesQActionRow rawRow)
		{
			var row = new RepositoriesModel();
			var objectRow = rawRow.ToObjectArray();
			foreach (var map in _columnMaps)
			{
				map.Apply(row, objectRow[map.Column.ColumnIndex]);
			}

			return row;
		}

		public RepositoriesQActionRow ToRawValue(RepositoriesModel row)
		{
			var qactionRow = new RepositoriesQActionRow();
			var rawRow = new object[qactionRow.ColumnCount];
			foreach (var map in _columnWriteMaps)
			{
				rawRow[map.Column.ColumnIndex] = map.GetRaw(row);
			}

			return new RepositoriesQActionRow(rawRow);
		}
	}

	public class RepositoriesModel
	{
		public string FullName { get; set; }

		public string Name { get; set; }

		public bool? Private { get; set; }

		public string Description { get; set; }

		public string Owner { get; set; }

		public bool? Fork { get; set; }

		public DateTime? CreatedAt { get; set; }

		public DateTime? UpdatedAt { get; set; }

		public DateTime? PushedAt { get; set; }

		public long? Size { get; set; }

		public int? Stars { get; set; }

		public int? Watcher { get; set; }

		public string Language { get; set; }

		public string DefaultBranch { get; set; }

		public RepositoryType? Type { get; set; }

		public string PublicKeyID { get; set; }

		public string PublicKey { get; set; }

		public long? Id { get; set; }

		public List<string> Topics { get; set; } = new List<string>();

		public bool? AutoRemove { get; set; }

		public static RepositoryType GetTypeFromTopics(IEnumerable<string> topics)
		{
			if (topics.Contains("dataminer-automation-script"))
			{
				return RepositoryType.Automation;
			}
			else if (topics.Contains("dataminer-connector"))
			{
				return RepositoryType.Connector;
			}
			else
			{
				return RepositoryType.Other;
			}
		}
	}

	public class RepositoriesQActionTable : SLTable<RepositoriesQActionRow>
	{
		public static readonly RepositoriesQActionTable Singleton = new RepositoriesQActionTable();

		protected RepositoriesQActionTable()
			: base(Parameter.Repositories.tablePid, Parameter.Repositories.indexColumn, Parameter.Repositories.indexColumnPid)
		{
			PollManagerQActionTable.Singleton.StateChanged += PollManager_StateChanged;

			FullName = new SLReadColumn<string>(
				Parameter.Repositories.Idx.repositoriesfullname,
				Parameter.Repositories.Pid.repositoriesfullname,
				this);

			Name = new SLReadColumn<string>(
				Parameter.Repositories.Idx.repositoriesname,
				Parameter.Repositories.Pid.repositoriesname,
				this);

			Private = new SLReadColumn<bool?>(
				Parameter.Repositories.Idx.repositoriesprivate,
				Parameter.Repositories.Pid.repositoriesprivate,
				this);

			Description = new SLReadColumn<string>(
				Parameter.Repositories.Idx.repositoriesdescription,
				Parameter.Repositories.Pid.repositoriesdescription,
				this);

			Owner = new SLReadColumn<string>(
				Parameter.Repositories.Idx.repositoriesowner,
				Parameter.Repositories.Pid.repositoriesowner,
				this);

			Fork = new SLReadColumn<bool?>(
				Parameter.Repositories.Idx.repositoriesfork,
				Parameter.Repositories.Pid.repositoriesfork,
				this);

			CreatedAt = new SLReadColumn<DateTime?>(
				Parameter.Repositories.Idx.repositoriescreatedat,
				Parameter.Repositories.Pid.repositoriescreatedat,
				this);

			UpdatedAt = new SLReadColumn<DateTime?>(
				Parameter.Repositories.Idx.repositoriesupdatedat,
				Parameter.Repositories.Pid.repositoriesupdatedat,
				this);

			PushedAt = new SLReadColumn<DateTime?>(
				Parameter.Repositories.Idx.repositoriespushedat,
				Parameter.Repositories.Pid.repositoriespushedat,
				this);

			Size = new SLReadColumn<long?>(
				Parameter.Repositories.Idx.repositoriessize,
				Parameter.Repositories.Pid.repositoriessize,
				this);

			Stars = new SLReadColumn<int?>(
				Parameter.Repositories.Idx.repositoriesstars,
				Parameter.Repositories.Pid.repositoriesstars,
				this);

			Watcher = new SLReadColumn<int?>(
				Parameter.Repositories.Idx.repositorieswatcher,
				Parameter.Repositories.Pid.repositorieswatcher,
				this);

			Language = new SLReadColumn<string>(
				Parameter.Repositories.Idx.repositorieslanguage,
				Parameter.Repositories.Pid.repositorieslanguage,
				this);

			DefaultBranch = new SLReadColumn<string>(
				Parameter.Repositories.Idx.repositoriesdefaultbranch,
				Parameter.Repositories.Pid.repositoriesdefaultbranch,
				this);

			Type = new SLReadColumn<RepositoryType?>(
				Parameter.Repositories.Idx.repositoriestype,
				Parameter.Repositories.Pid.repositoriestype,
				new RepositoryTypeConverter(),
				this);

			PublicKeyID = new SLReadColumn<string>(
				Parameter.Repositories.Idx.repositoriespublickeyid,
				Parameter.Repositories.Pid.repositoriespublickeyid,
				this);

			PublicKey = new SLReadColumn<string>(
				Parameter.Repositories.Idx.repositoriespublickey,
				Parameter.Repositories.Pid.repositoriespublickey,
				this);

			Id = new SLReadColumn<long?>(
				Parameter.Repositories.Idx.repositoriesid,
				Parameter.Repositories.Pid.repositoriesid,
				this);

			Topics = new SLReadColumn<List<string>>(
				Parameter.Repositories.Idx.repositoriestopics,
				Parameter.Repositories.Pid.repositoriestopics,
				new TopicsConverter(),
				this);

			AutoRemove = new SLReadWriteColumn<bool?, bool?>(
				Parameter.Repositories.Idx.repositoriesautoremove,
				Parameter.Repositories.Pid.repositoriesautoremove,
				Parameter.Write.repositoriesautoremove_1120,
				this);
		}

		public SLReadColumn<string> FullName { get; }

		public SLReadColumn<string> Name { get; }

		public SLReadColumn<bool?> Private { get; }

		public SLReadColumn<string> Description { get; }

		public SLReadColumn<string> Owner { get; }

		public SLReadColumn<bool?> Fork { get; }

		public SLReadColumn<DateTime?> CreatedAt { get; }

		public SLReadColumn<DateTime?> UpdatedAt { get; }

		public SLReadColumn<DateTime?> PushedAt { get; }

		public SLReadColumn<long?> Size { get; }

		public SLReadColumn<int?> Stars { get; }

		public SLReadColumn<int?> Watcher { get; }

		public SLReadColumn<string> Language { get; }

		public SLReadColumn<string> DefaultBranch { get; }

		public SLReadColumn<RepositoryType?> Type { get; }

		public SLReadColumn<string> PublicKeyID { get; }

		public SLReadColumn<string> PublicKey { get; }

		public SLReadColumn<long?> Id { get; }

		public SLReadColumn<List<string>> Topics { get; }

		public SLReadWriteColumn<bool?, bool?> AutoRemove { get; }

		protected override void DisposeEvents()
		{
			PollManagerQActionTable.Singleton.StateChanged -= PollManager_StateChanged;
		}

		private void PollManager_StateChanged(object sender, PollManagerStateChangedEventArgs e)
		{
			if (e.RequestType == RequestType.Repositories_PublicKey &&
				e.PollState != PollState.Disabled)
			{
				PublicKey_Disabled(e.Protocol);
				return;
			}

			if (e.RequestType == RequestType.Organizations_Repositories &&
				e.PollState != PollState.Disabled)
			{
				OrganizationsRepositories_Disabled(e.Protocol);
				return;
			}

			if (e.RequestType == RequestType.Repositories_Repositories &&
				e.PollState != PollState.Disabled)
			{
				Repositories_Disabled(e.Protocol);
			}
		}

		private void PublicKey_Disabled(SLProtocol protocol)
		{
			var models = SLTables.Repositories.GetPrimaryKeys(protocol)
				.Select(key => new PublicKeyModel { PrimaryKey = key, PublicKey = Exceptions.NotAvailable, PublicKeyID = Exceptions.NotAvailable });
			SLTables.Repositories.SetData(
				protocol,
				models,
				m => m.PrimaryKey,
				SLTables.Repositories.PublicKeyID.Read.MapWrite<PublicKeyModel>(m => m.PublicKeyID),
				SLTables.Repositories.PublicKey.Read.MapWrite<PublicKeyModel>(m => m.PublicKey));
		}

		private void Repositories_Disabled(SLProtocol protocol)
		{
			// Get all the tracked organizations
			var trackedOrganizations = SLTables.Organizations.GetData(
				protocol,
				SLTables.Organizations.Instance.Read.Map<OrganizationsModel>(m => m.Instance),
				SLTables.Organizations.Tracked.Read.Map<OrganizationsModel>(m => m.Tracked))
					.Where(m => m.Tracked.HasValue && m.Tracked.Value)
					.Select(m => m.Instance)
					.ToHashSet();

			// Get all the non manually added repositories for the tracked organization and remove them.
			// As they are no longer polled
			var toBeUpdated = SLTables.Repositories.GetData(
				protocol,
				SLTables.Repositories.FullName.Read.Map<RepositoriesModel>(m => m.FullName),
				SLTables.Repositories.Owner.Read.Map<RepositoriesModel>(m => m.Owner),
				SLTables.Repositories.PublicKey.Read.Map<RepositoriesModel>(m => m.PublicKey),
				SLTables.Repositories.PublicKeyID.Read.Map<RepositoriesModel>(m => m.PublicKeyID),
				SLTables.Repositories.AutoRemove.Read.Map<RepositoriesModel>(m => m.AutoRemove))
					.Where(m => m.AutoRemove.HasValue && m.AutoRemove.Value)    // If manually added
					.Where(m => trackedOrganizations.Contains(m.Owner))         // And not polled via the tracked organization
					.Select(m => RepositoriesRowConverter.Instance.ToRawValue(m))
					.Select(m =>
					{
						return new RepositoriesQActionRow
						{
							Repositoriesfullname_1001 = m.Repositoriesfullname,
							Repositoriesname_1002 = Exceptions.NotAvailable,
							Repositoriesprivate_1003 = Exceptions.IntNotAvailable,
							Repositoriesdescription_1004 = Exceptions.NotAvailable,
							Repositoriesowner_1005 = Exceptions.NotAvailable,
							Repositoriesfork_1006 = Exceptions.IntNotAvailable,
							Repositoriescreatedat_1007 = Exceptions.IntNotAvailable,
							Repositoriesupdatedat_1008 = Exceptions.IntNotAvailable,
							Repositoriespushedat_1009 = Exceptions.IntNotAvailable,
							Repositoriessize_1010 = Exceptions.IntNotAvailable,
							Repositoriesstars_1011 = Exceptions.IntNotAvailable,
							Repositorieswatcher_1012 = Exceptions.IntNotAvailable,
							Repositorieslanguage_1013 = Exceptions.NotAvailable,
							Repositoriesdefaultbranch_1014 = Exceptions.NotAvailable,
							Repositoriestype_1015 = Exceptions.IntNotAvailable,
							Repositoriespublickeyid_1016 = m.Repositoriespublickeyid_1016,
							Repositoriespublickey_1017 = m.Repositoriespublickey_1017,
							Repositoriesid_1018 = Exceptions.IntNotAvailable,
							Repositoriestopics_1019 = Exceptions.NotAvailable,
							Repositoriesautoremove_1020 = m.Repositoriesautoremove_1020,
						};
					})
					.ToList();

			// We'll set them to N/A since they were manually added.
			// This way we don't lose the information that was added manually.
			SLTables.Repositories.FillTableNoDelete(protocol, toBeUpdated);
		}

		private void OrganizationsRepositories_Disabled(SLProtocol protocol)
		{
			// Get all the tracked organizations
			var trackedOrganizations = SLTables.Organizations.GetData(
				protocol,
				SLTables.Organizations.Instance.Read.Map<OrganizationsModel>(m => m.Instance),
				SLTables.Organizations.Tracked.Read.Map<OrganizationsModel>(m => m.Tracked))
					.Where(m => m.Tracked.HasValue && m.Tracked.Value)
					.Select(m => m.Instance)
					.ToHashSet();

			// Get all the non manually added repositories for the tracked organization and remove them.
			// As they are no longer polled
			var toBeRemoved = SLTables.Repositories.GetData(
				protocol,
				SLTables.Repositories.FullName.Read.Map<RepositoriesModel>(m => m.FullName),
				SLTables.Repositories.Owner.Read.Map<RepositoriesModel>(m => m.Owner),
				SLTables.Repositories.AutoRemove.Read.Map<RepositoriesModel>(m => m.AutoRemove))
					.Where(m => m.AutoRemove.HasValue && m.AutoRemove.Value)
					.Where(m => trackedOrganizations.Contains(m.Owner))
					.Select(m => m.FullName)
					.ToHashSet();

			SLTables.Repositories.DeleteRows(protocol, toBeRemoved);
		}

		private sealed class PublicKeyModel
		{
			public string PrimaryKey { get; set; }

			public string PublicKeyID { get; set; }

			public string PublicKey { get; set; }
		}
	}
}
