// Ignore Spelling: Pids

namespace Skyline.Protocol.PollManager
{
	using System;

	[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
	public sealed class TableAttribute : Attribute
	{
		public TableAttribute(params int[] tablePids)
		{
			TableIDs = tablePids;
		}

		public int[] TableIDs { get; set; }
	}
}
