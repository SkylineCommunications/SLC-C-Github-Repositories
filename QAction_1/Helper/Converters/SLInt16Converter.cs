namespace Skyline.DataMiner.Scripting.Helper.Converters
{
	using System;

	internal class SLInt16Converter : ISLConverter<Int16?>
	{
		public Int16? FromRawValue(object rawValue)
		{
			if (rawValue != null)
				return Convert.ToInt16(rawValue);

			return null;
		}

		public object ToRawValue(Int16? value)
		{
			return value ?? (object)null;
		}
	}
}