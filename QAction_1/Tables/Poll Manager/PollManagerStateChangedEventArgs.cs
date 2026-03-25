namespace Skyline.Protocol.Tables.Poll_Manager
{
	using Skyline.DataMiner.Scripting;
	using Skyline.DataMiner.Scripting.Helper.Events;
	using Skyline.Protocol.PollManager;

	public class PollManagerStateChangedEventArgs : TableEventArgs
	{
		public PollManagerStateChangedEventArgs(SLProtocol protocol, RequestType requestType, PollState pollState)
			: base(protocol)
		{
			RequestType = requestType;
			PollState = pollState;
		}

		public RequestType RequestType { get; }

		public PollState PollState { get; }
	}
}
