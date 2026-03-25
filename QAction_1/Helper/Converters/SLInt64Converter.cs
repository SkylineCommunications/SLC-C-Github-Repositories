namespace Skyline.DataMiner.Scripting.Helper.Converters
{
	using System;

	public class SLInt64Converter : ISLConverter<Int64?>
	{
		public Int64? FromRawValue(object rawValue)
		{
			if (rawValue != null)
				return Convert.ToInt64(rawValue);

			return null;
		}

		public object ToRawValue(Int64? value)
		{
			return value ?? (object)null;
		}
	}
}