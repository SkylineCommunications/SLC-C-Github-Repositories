namespace Skyline.Protocol.Tables
{
    public enum RepositoryTableAction
    {
        Add,
        Remove,
    }

    public enum IndividualOrOrganization
    {
        Individual,
        Organization,
    }

    public interface IRepositoriesTableRequest
    {
        RepositoryTableAction Action { get; }

        IndividualOrOrganization Organization { get; }

        string[] Data { get; }
    }
}
