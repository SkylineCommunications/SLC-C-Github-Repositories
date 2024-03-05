// Ignore Spelling: Pid

namespace Skyline.Protocol.PollManager
{
	using System;

	[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
	public sealed class TableAttribute : Attribute
	{
		public TableAttribute(int tablePid)
		{
			TableID = tablePid;
		}

		public int TableID { get; set; }
	}
}
