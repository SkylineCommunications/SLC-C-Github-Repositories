namespace Skyline.Protocol.PollManager.RequestHandler.Organizations
{
    using Skyline.DataMiner.Scripting;

    public static partial class OrganizationsRequestHandler
    {
        public static void HandleUserOrganizationsRequest(SLProtocol protocol)
        {
            protocol.CheckTrigger(210);
        }
    }
}
