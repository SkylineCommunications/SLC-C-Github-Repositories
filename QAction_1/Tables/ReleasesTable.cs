namespace Skyline.Protocol.Tables
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Skyline.DataMiner.Scripting;

    using SLNetMessages = Skyline.DataMiner.Net.Messages;

    public class RepositoryReleasesRecord
    {
        private double isDraft = Exceptions.IntNotAvailable;
        private double isPreRelease = Exceptions.IntNotAvailable;
        private DateTime createdAt;
        private double createdAtOA = Exceptions.IntNotAvailable;
        private DateTime publishedAt;
        private double publishedAtOA = Exceptions.IntNotAvailable;

        public RepositoryReleasesRecord() { }

        public RepositoryReleasesRecord(params object[] row)
        {
            Instance = Convert.ToString(row[0]);
            ID = Convert.ToInt64(row[1]);
            TagName = Convert.ToString(row[2]);
            TagId = Convert.ToString(row[3]);
            TargetCommitish = Convert.ToString(row[4]);
            Name = Convert.ToString(row[5]);
            Draft = Convert.ToBoolean(row[6]);
            PreRelease = Convert.ToBoolean(row[7]);
            Body = Convert.ToString(row[8]);
            Author = Convert.ToString(row[9]);
            CreatedAt = DateTime.FromOADate(Convert.ToDouble(row[10]));
            PublishedAt = DateTime.FromOADate(Convert.ToDouble(row[11]));
            RepositoryId = Convert.ToString(row[12]);
        }

        public string Instance { get; set; }

        public long ID { get; set; } = Exceptions.IntNotAvailable;

        public string TagName { get; set; } = Exceptions.NotAvailable;

        public string TagId { get; set; } = Exceptions.NotAvailable;

        public string TargetCommitish { get; set; } = Exceptions.NotAvailable;

        public string Name { get; set; } = Exceptions.NotAvailable;

        public bool Draft { get => Convert.ToBoolean(isDraft); set => isDraft = Convert.ToDouble(value); }

        public bool PreRelease { get => Convert.ToBoolean(isPreRelease); set => isPreRelease = Convert.ToDouble(value); }

        public string Body { get; set; } = Exceptions.NotAvailable;

        public string Author { get; set; } = Exceptions.NotAvailable;

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

        public DateTime PublishedAt
        {
            get
            {
                return publishedAt;
            }

            set
            {
                publishedAt = value;
                publishedAtOA = value.ToOADate();
            }
        }

        public string RepositoryId { get; set; } = Exceptions.NotAvailable;

        public static RepositoryReleasesRecord FromPK(SLProtocol protocol, string pk)
        {
            var row = (object[])protocol.GetRow(Parameter.Repositoryreleases.tablePid, pk);
            if (row[0] == null)
            {
                return default;
            }

            return new RepositoryReleasesRecord(row);
        }

        public object[] ToProtocolRow()
        {
            return new RepositoryreleasesQActionRow
            {
                Repositoryreleasesinstance = Instance,
                Repositoryreleasesid = ID,
                Repositoryreleasestagname = TagName,
                Repositoryreleasestagid = TagId,
                Repositoryreleasestargetcommitish = TargetCommitish,
                Repositoryreleasesname = Name,
                Repositoryreleasesdraft = isDraft,
                Repositoryreleasesprerelease = isPreRelease,
                Repositoryreleasesbody = Body,
                Repositoryreleasesauthor = Author,
                Repositoryreleasescreatedat = createdAtOA,
                Repositoryreleasespublishedat = publishedAtOA,
                Repositoryreleasesrepositoryid = RepositoryId,
            };
        }

        public void SaveToProtocol(SLProtocol protocol)
        {
            if (!protocol.Exists(Parameter.Repositoryreleases.tablePid, Instance))
            {
                protocol.AddRow(Parameter.Repositoryreleases.tablePid, ToProtocolRow());
            }
            else
            {
                protocol.SetRow(Parameter.Repositoryreleases.tablePid, Instance, ToProtocolRow());
            }
        }
    }

    public class RepositoryReleasesRecords
    {
        public RepositoryReleasesRecords() { { } }

        public RepositoryReleasesRecords(SLProtocol protocol)
        {
            uint[] repositoryReleasesIdx = new uint[]
            {
                Parameter.Repositoryreleases.Idx.repositoryreleasesinstance,
                Parameter.Repositoryreleases.Idx.repositoryreleasesid,
                Parameter.Repositoryreleases.Idx.repositoryreleasestagname,
                Parameter.Repositoryreleases.Idx.repositoryreleasestagid,
                Parameter.Repositoryreleases.Idx.repositoryreleasestargetcommitish,
                Parameter.Repositoryreleases.Idx.repositoryreleasesname,
                Parameter.Repositoryreleases.Idx.repositoryreleasesdraft,
                Parameter.Repositoryreleases.Idx.repositoryreleasesprerelease,
                Parameter.Repositoryreleases.Idx.repositoryreleasesbody,
                Parameter.Repositoryreleases.Idx.repositoryreleasesauthor,
                Parameter.Repositoryreleases.Idx.repositoryreleasescreatedat,
                Parameter.Repositoryreleases.Idx.repositoryreleasespublishedat,
                Parameter.Repositoryreleases.Idx.repositoryreleasesrepositoryid,
            };
            object[] repositoryreleases = (object[])protocol.NotifyProtocol((int)SLNetMessages.NotifyType.NT_GET_TABLE_COLUMNS, Parameter.Repositoryreleases.tablePid, repositoryReleasesIdx);
            object[] instance = (object[])repositoryreleases[0];
            object[] iD = (object[])repositoryreleases[1];
            object[] tagName = (object[])repositoryreleases[2];
            object[] tagId = (object[])repositoryreleases[3];
            object[] targetCommitish = (object[])repositoryreleases[4];
            object[] name = (object[])repositoryreleases[5];
            object[] draft = (object[])repositoryreleases[6];
            object[] preRelease = (object[])repositoryreleases[7];
            object[] body = (object[])repositoryreleases[8];
            object[] author = (object[])repositoryreleases[9];
            object[] createdAt = (object[])repositoryreleases[10];
            object[] publishedAt = (object[])repositoryreleases[11];
            object[] repositoryId = (object[])repositoryreleases[12];

            Rows = new List<RepositoryReleasesRecord>();
            for (int i = 0; i < instance.Length; i++)
            {
                Rows.Add(new RepositoryReleasesRecord(
                instance[i],
                iD[i],
                tagName[i],
                tagId[i],
                targetCommitish[i],
                name[i],
                draft[i],
                preRelease[i],
                body[i],
                author[i],
                createdAt[i],
                publishedAt[i],
                repositoryId[i]));
            }
        }

        public List<RepositoryReleasesRecord> Rows { get; set; }

        public void SaveToProtocol(SLProtocol protocol, bool partial = false)
        {
            List<object[]> rows = Rows.Select(x => x.ToProtocolRow()).ToList();
            NotifyProtocol.SaveOption option = partial ? NotifyProtocol.SaveOption.Partial : NotifyProtocol.SaveOption.Full;
            protocol.FillArray(Parameter.Repositoryreleases.tablePid, rows, option);
        }
    }
}
