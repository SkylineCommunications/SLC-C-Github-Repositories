namespace Skyline.Protocol.Tables
{
	using System;

	using Skyline.DataMiner.Scripting.Helper;
	using Skyline.Protocol.PollManager;

	internal class RequestTypeConverter : ISLConverter<RequestType?>
	{
		public RequestType? FromRawValue(object rawValue)
		{
			if (rawValue is null)
			{
				return null;
			}

			return (RequestType)Convert.ToInt32(rawValue);
		}

		public object ToRawValue(RequestType? value)
		{
			if (!value.HasValue)
			{
				return null;
			}

			return Convert.ToString((int)value);
		}
	}
}
