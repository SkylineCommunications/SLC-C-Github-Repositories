namespace Skyline.DataMiner.Scripting.Helper.Converters
{
	using System;

	public class SLUInt64Converter : ISLConverter<UInt64?>
	{
		public UInt64? FromRawValue(object rawValue)
		{
			if (rawValue != null)
				return Convert.ToUInt64(rawValue);

			return null;
		}

		public object ToRawValue(UInt64? value)
		{
			return value ?? (object)null;
		}
	}
}