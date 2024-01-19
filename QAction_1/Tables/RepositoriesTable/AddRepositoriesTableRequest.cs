namespace Skyline.Protocol.Tables
{
    public class AddRepositoriesTableRequest : IRepositoriesTableRequest
    {
        public AddRepositoriesTableRequest(string owner, string name)
        {
            Action = RepositoryTableAction.Add;
            Organization = IndividualOrOrganization.Individual;
            Data = new[] { owner, name };
        }

        public AddRepositoriesTableRequest(string organization)
        {
            Action = RepositoryTableAction.Add;
            Organization = IndividualOrOrganization.Organization;
            Data = new[] { organization };
        }

        public RepositoryTableAction Action { get; }

        public IndividualOrOrganization Organization { get; }

        public string[] Data { get; }
    }
}
