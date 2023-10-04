namespace Skyline.Protocol.Tables
{
    public enum RepositoryTableAction
    {
        Add,
        Remove,
    }

    public interface IRepositoriesTableRequest
    {
        RepositoryTableAction Action { get; }

        string[] Data { get; }
    }
}
