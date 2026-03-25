namespace Skyline.DataMiner.Scripting.Helper.Converters
{
	using System;

	public class SLUInt16Converter : ISLConverter<UInt16?>
	{
		public UInt16? FromRawValue(object rawValue)
		{
			if (rawValue != null)
				return Convert.ToUInt16(rawValue);

			return null;
		}

		public object ToRawValue(UInt16? value)
		{
			return value ?? (object)null;
		}
	}
}