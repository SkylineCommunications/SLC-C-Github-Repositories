namespace Skyline.Protocol.Tables.Poll_Manager
{
	using System;

	using Skyline.DataMiner.Scripting.Helper;
	using Skyline.Protocol.PollManager;

	internal class PollingStatusConverter : ISLConverter<PollingStatus?>
	{
		public PollingStatus? FromRawValue(object rawValue)
		{
			if (rawValue is null)
			{
				return null;
			}

			return (PollingStatus)Convert.ToInt32(rawValue);
		}

		public object ToRawValue(PollingStatus? value)
		{
			if (!value.HasValue)
			{
				return null;
			}

			return Convert.ToInt32(value);
		}
	}
}
