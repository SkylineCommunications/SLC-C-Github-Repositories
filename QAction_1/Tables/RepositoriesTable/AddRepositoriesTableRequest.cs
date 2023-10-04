namespace Skyline.Protocol.Tables
{
    public class AddRepositoriesTableRequest : IRepositoriesTableRequest
    {
        public AddRepositoriesTableRequest(string owner, string name)
        {
            Action = RepositoryTableAction.Add;
            Data = new[] { owner, name };
        }

        public RepositoryTableAction Action { get; }

        public string[] Data { get; }
    }
}
