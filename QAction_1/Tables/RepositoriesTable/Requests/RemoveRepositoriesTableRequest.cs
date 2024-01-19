namespace Skyline.Protocol.Tables
{
    public class RemoveRepositoriesTableRequest : IRepositoriesTableRequest
    {
        public RemoveRepositoriesTableRequest(IndividualOrOrganization organization, params string[] keys)
        {
            Action = RepositoryTableAction.Remove;
            Organization = organization;
            Data = keys;
        }

        public RepositoryTableAction Action { get; }

        public IndividualOrOrganization Organization { get; }

        public string[] Data { get; }
    }
}
