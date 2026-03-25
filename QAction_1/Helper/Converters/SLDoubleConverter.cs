namespace Skyline.DataMiner.Scripting.Helper.Converters
{
	using System;

	public class SLDoubleConverter : ISLConverter<double?>
	{
		public double? FromRawValue(object rawValue)
		{
			if (rawValue != null)
				return Convert.ToDouble(rawValue);

			return null;
		}

		public object ToRawValue(double? value)
		{
			return value ?? (object)null;
		}
	}
}