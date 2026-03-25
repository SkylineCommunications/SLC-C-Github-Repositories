namespace Skyline.DataMiner.Scripting.Helper.Converters
{
	using System;

	public class SLInt32Converter : ISLConverter<Int32?>
	{
		public Int32? FromRawValue(object rawValue)
		{
			if (rawValue != null)
				return Convert.ToInt32(rawValue);

			return null;
		}

		public object ToRawValue(Int32? value)
		{
			return value ?? (object)null;
		}
	}
}