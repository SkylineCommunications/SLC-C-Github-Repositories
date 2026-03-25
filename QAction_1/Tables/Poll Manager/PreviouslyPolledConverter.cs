namespace Skyline.Protocol.Tables.Poll_Manager
{
	using System;

	using Skyline.DataMiner.Scripting.Helper;

	internal class PreviouslyPolledConverter : ISLConverter<DateTime?>
	{
		public DateTime? FromRawValue(object rawValue)
		{
			if (rawValue is null)
			{
				return null;
			}

			var doubleValue = Convert.ToDouble(rawValue);
			if (DoubleIntEqual(doubleValue, 0, 0.1))
			{
				return null;
			}

			return DateTime.SpecifyKind(DateTime.FromOADate(Convert.ToDouble(rawValue)), DateTimeKind.Utc);
		}

		public object ToRawValue(DateTime? value)
		{
			if (!value.HasValue)
			{
				return 0;
			}

			return value.Value.ToOADate();
		}

		private static bool DoubleIntEqual(double d, int i, double epsilon = Double.Epsilon)
		{
			return Math.Abs(d - i) < epsilon;
		}
	}
}
