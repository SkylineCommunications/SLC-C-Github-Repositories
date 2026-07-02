namespace Skyline.DataMiner.Scripting.Helper.Converters
{
	using System;

	public class SLTimeSpanConverter : ISLConverter<TimeSpan?>
	{
		public TimeSpan? FromRawValue(object rawValue)
		{
			if (rawValue != null)
			{
				return TimeSpan.FromSeconds(Convert.ToDouble(rawValue));
			}

			return null;
		}

		public object ToRawValue(TimeSpan? value)
		{
			if (value != null)
			{
				return value.Value.TotalSeconds;
			}

			return null;
		}
	}
}