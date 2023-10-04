namespace Skyline.Protocol.Tables
{
    public class RemoveRepositoriesTableRequest : IRepositoriesTableRequest
    {
        public RemoveRepositoriesTableRequest(params string[] keys)
        {
            Action = RepositoryTableAction.Remove;
            Data = keys;
        }

        public RepositoryTableAction Action { get; }

        public string[] Data { get; }
    }
}
