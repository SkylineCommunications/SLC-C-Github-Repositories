namespace Skyline.Protocol.Tables
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Skyline.DataMiner.Scripting;

    using SLNetMessages = Skyline.DataMiner.Net.Messages;

    public class RepositoryTagsRecord
    {
        public RepositoryTagsRecord() { }

        public RepositoryTagsRecord(params object[] row)
        {
            ID = Convert.ToString(row[0]);
            Name = Convert.ToString(row[1]);
            RepositoryID = Convert.ToString(row[2]);
            CommitSHA = Convert.ToString(row[3]);
        }

        public string ID { get; set; }

        public string Name { get; set; }

        public string RepositoryID { get; set; }

        public string CommitSHA { get; set; }

        public static RepositoryTagsRecord FromPK(SLProtocol protocol, string pk)
        {
            var row = (object[])protocol.GetRow(Parameter.Repositorytags.tablePid, pk);
            if (row[0] == null)
            {
                return default;
            }

            return new RepositoryTagsRecord(row);
        }

        public object[] ToProtocolRow()
        {
            return new RepositorytagsQActionRow
            {
                Repositorytagsid_1201 = ID,
                Repositorytagsname_1202 = Name,
                Repositorytagsrepositoryid_1203 = RepositoryID,
                Repositorytagscommitsha_1204 = CommitSHA,
            };
        }

        public void SaveToProtocol(SLProtocol protocol)
        {
            if (!protocol.Exists(Parameter.Repositorytags.tablePid, ID))
            {
                protocol.AddRow(Parameter.Repositorytags.tablePid, ToProtocolRow());
            }
            else
            {
                protocol.SetRow(Parameter.Repositorytags.tablePid, ID, ToProtocolRow());
            }
        }
    }

    public class RepositoryTagsRecords
    {
        public RepositoryTagsRecords() { { } }

        public RepositoryTagsRecords(SLProtocol protocol)
        {
            uint[] repositoryTagsIdx = new uint[]
            {
                Parameter.Repositorytags.Idx.repositorytagsid_1201,
                Parameter.Repositorytags.Idx.repositorytagsname_1202,
                Parameter.Repositorytags.Idx.repositorytagsrepositoryid_1203,
                Parameter.Repositorytags.Idx.repositorytagscommitsha_1204,
            };
            object[] repositorytags = (object[])protocol.NotifyProtocol((int)SLNetMessages.NotifyType.NT_GET_TABLE_COLUMNS, Parameter.Repositorytags.tablePid, repositoryTagsIdx);
            object[] iD = (object[])repositorytags[0];
            object[] name = (object[])repositorytags[1];
            object[] repositoryID = (object[])repositorytags[2];
            object[] commitSHA = (object[])repositorytags[3];

            Rows = new List<RepositoryTagsRecord>();
            for (int i = 0; i < iD.Length; i++)
            {
                Rows.Add(new RepositoryTagsRecord(
                iD[i],
                name[i],
                repositoryID[i],
                commitSHA[i]));
            }
        }

        public List<RepositoryTagsRecord> Rows { get; set; }

        public void SaveToProtocol(SLProtocol protocol, bool partial = false)
        {
            List<object[]> rows = Rows.Select(x => x.ToProtocolRow()).ToList();
            NotifyProtocol.SaveOption option = partial ? NotifyProtocol.SaveOption.Partial : NotifyProtocol.SaveOption.Full;
            protocol.FillArray(Parameter.Repositorytags.tablePid, rows, option);
        }
    }
}
