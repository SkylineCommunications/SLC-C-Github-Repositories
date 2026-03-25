namespace Skyline.Protocol.Tables
{
	using System;

	using Skyline.DataMiner.Scripting.Helper;
	using Skyline.Protocol.PollManager;

	internal class PollStateConverter : ISLConverter<PollState?>
	{
		public PollState? FromRawValue(object rawValue)
		{
			if (rawValue is null)
			{
				return null;
			}

			return (PollState)Convert.ToInt32(rawValue);
		}

		public object ToRawValue(PollState? value)
		{
			if (!value.HasValue)
			{
				return null;
			}

			return Convert.ToInt32(value);
		}
	}
}
