namespace Skyline.DataMiner.Scripting.Helper.Converters
{
	using System;

	internal class SLDateTimeConverter : ISLConverter<DateTime?>
	{
		public DateTime? FromRawValue(object rawValue)
		{
			if (rawValue != null)
			{
				return DateTime.SpecifyKind(DateTime.FromOADate(Convert.ToDouble(rawValue)), DateTimeKind.Utc);
			}

			return null;
		}

		public object ToRawValue(DateTime? value)
		{
			if (value.HasValue)
			{
				return value.Value.ToOADate();
			}

			return null;
		}
	}
}