namespace Skyline.DataMiner.Scripting.Helper.Events
{
	using System;

	using Skyline.DataMiner.Scripting;

	public class TableEventArgs : EventArgs
	{
		public TableEventArgs(SLProtocol protocol)
		{
			Protocol = protocol ?? throw new ArgumentNullException(nameof(protocol));
		}

		public SLProtocol Protocol { get; }
	}
}
