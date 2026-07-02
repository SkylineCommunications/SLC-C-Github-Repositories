namespace Skyline.DataMiner.Scripting.Helper.Converters
{
	using System;

	public class SLUInt32Converter : ISLConverter<UInt32?>
	{
		public UInt32? FromRawValue(object rawValue)
		{
			if (rawValue != null)
			{
				return Convert.ToUInt32(rawValue);
			}

			return null;
		}

		public object ToRawValue(UInt32? value)
		{
			return value ?? (object)null;
		}
	}
}