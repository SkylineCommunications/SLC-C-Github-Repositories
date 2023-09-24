namespace Skyline.Protocol.Tables
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Skyline.DataMiner.Scripting;
    using Skyline.Protocol;

    using SLNetMessages = Skyline.DataMiner.Net.Messages;

    public class RepositoriesTableRow
    {
        private double isPrivate = Exceptions.IntNotAvailable;
        private double isFork = Exceptions.IntNotAvailable;
        private DateTime createdAt;
        private double createdAtOA = Exceptions.IntNotAvailable;
        private DateTime updatedAt;
        private double updatedAtOA = Exceptions.IntNotAvailable;
        private DateTime pushedAt;
        private double pushedAtOA = Exceptions.IntNotAvailable;

        public RepositoriesTableRow() { }

        public RepositoriesTableRow(params object[] row)
        {
            Name = Convert.ToString(row[0]);
            FullName = Convert.ToString(row[1]);
            Private = Convert.ToBoolean(row[2]);
            Description = Convert.ToString(row[3]);
            Owner = Convert.ToString(row[4]);
            Fork = Convert.ToBoolean(row[5]);
            CreatedAt = DateTime.FromOADate(Convert.ToDouble(row[6]));
            UpdatedAt = DateTime.FromOADate(Convert.ToDouble(row[7]));
            PushedAt = DateTime.FromOADate(Convert.ToDouble(row[8]));
            Size = Convert.ToInt64(row[9]);
            Stars = Convert.ToInt32(row[10]);
            Watcher = Convert.ToInt32(row[11]);
            Language = Convert.ToString(row[12]);
        }

        public string Name { get; set; }

        public string FullName { get; set; } = Exceptions.NotAvailable;

        public bool Private { get => Convert.ToBoolean(isPrivate); set => isPrivate = Convert.ToDouble(value); }

        public string Description { get; set; } = Exceptions.NotAvailable;

        public string Owner { get; set; } = Exceptions.NotAvailable;

        public bool Fork { get => Convert.ToBoolean(isFork); set => isFork = Convert.ToDouble(value); }

        public DateTime CreatedAt
        {
            get
            {
                return createdAt;
            }

            set
            {
                createdAt = value;
                createdAtOA = value.ToOADate();
            }
        }

        public DateTime UpdatedAt
        {
            get
            {
                return updatedAt;
            }

            set
            {
                updatedAt = value;
                updatedAtOA = value.ToOADate();
            }
        }

        public DateTime PushedAt
        {
            get
            {
                return pushedAt;
            }

            set
            {
                pushedAt = value;
                pushedAtOA = value.ToOADate();
            }
        }

        public long Size { get; set; } = Exceptions.IntNotAvailable;

        public int Stars { get; set; } = Exceptions.IntNotAvailable;

        public int Watcher { get; set; } = Exceptions.IntNotAvailable;

        public string Language { get; set; } = Exceptions.NotAvailable;

        public static RepositoriesTableRow FromPK(SLProtocol protocol, string pk)
        {
            var row = (object[])protocol.GetRow(Parameter.Repositories.tablePid, pk);
            if (row[0] == null)
            {
                return default;
            }

            return new RepositoriesTableRow(row);
        }

        public object[] ToProtocolRow()
        {
            return new RepositoriesQActionRow
            {
                Repositoriesname = Name,
                Repositoriesfullname = FullName,
                Repositoriesprivate = isPrivate,
                Repositoriesdescription = Description,
                Repositoriesowner = Owner,
                Repositoriesfork = isFork,
                Repositoriescreatedat = createdAtOA,
                Repositoriesupdatedat = updatedAtOA,
                Repositoriespushedat = pushedAtOA,
                Repositoriessize = Convert.ToDouble(Size),
                Repositoriesstars = Convert.ToDouble(Stars),
                Repositorieswatcher = Convert.ToDouble(Watcher),
                Repositorieslanguage = Language,
            };
        }

        public void SaveToProtocol(SLProtocol protocol)
        {
            if (!protocol.Exists(Parameter.Repositories.tablePid, Name))
            {
                protocol.AddRow(Parameter.Repositories.tablePid, ToProtocolRow());
            }
            else
            {
                protocol.SetRow(Parameter.Repositories.tablePid, Name, ToProtocolRow());
            }
        }
    }

    public class RepositoriesTable
    {
        public RepositoriesTable() { }

        public RepositoriesTable(SLProtocol protocol)
        {
            uint[] repositoriesTableIdx = new uint[]
            {
                Parameter.Repositories.Idx.repositoriesname,
                Parameter.Repositories.Idx.repositoriesfullname,
                Parameter.Repositories.Idx.repositoriesprivate,
                Parameter.Repositories.Idx.repositoriesdescription,
                Parameter.Repositories.Idx.repositoriesowner,
                Parameter.Repositories.Idx.repositoriesfork,
                Parameter.Repositories.Idx.repositoriescreatedat,
                Parameter.Repositories.Idx.repositoriesupdatedat,
                Parameter.Repositories.Idx.repositoriespushedat,
                Parameter.Repositories.Idx.repositoriessize,
                Parameter.Repositories.Idx.repositoriesstars,
                Parameter.Repositories.Idx.repositorieswatcher,
                Parameter.Repositories.Idx.repositorieslanguage,
            };
            object[] repositoriestable = (object[])protocol.NotifyProtocol((int)SLNetMessages.NotifyType.NT_GET_TABLE_COLUMNS, Parameter.Repositories.tablePid, repositoriesTableIdx);
            object[] name = (object[])repositoriestable[0];
            object[] fullName = (object[])repositoriestable[1];
            object[] isprivate = (object[])repositoriestable[2];
            object[] description = (object[])repositoriestable[3];
            object[] owner = (object[])repositoriestable[4];
            object[] fork = (object[])repositoriestable[5];
            object[] createdAt = (object[])repositoriestable[6];
            object[] updatedAt = (object[])repositoriestable[7];
            object[] pushedAt = (object[])repositoriestable[8];
            object[] size = (object[])repositoriestable[9];
            object[] stars = (object[])repositoriestable[10];
            object[] watcher = (object[])repositoriestable[11];
            object[] language = (object[])repositoriestable[12];

            Rows = new List<RepositoriesTableRow>();
            for (int i = 0; i < name.Length; i++)
            {
                Rows.Add(new RepositoriesTableRow(
                name[i],
                fullName[i],
                isprivate[i],
                description[i],
                owner[i],
                fork[i],
                createdAt[i],
                updatedAt[i],
                pushedAt[i],
                size[i],
                stars[i],
                watcher[i],
                language[i]));
            }
        }

        public List<RepositoriesTableRow> Rows { get; set; }

        public void SaveToProtocol(SLProtocol protocol, bool partial = false)
        {
            List<object[]> rows = Rows.Select(x => x.ToProtocolRow()).ToList();
            NotifyProtocol.SaveOption option = partial ? NotifyProtocol.SaveOption.Partial : NotifyProtocol.SaveOption.Full;
            protocol.FillArray(Parameter.Repositories.tablePid, rows, option);
        }
    }
}
