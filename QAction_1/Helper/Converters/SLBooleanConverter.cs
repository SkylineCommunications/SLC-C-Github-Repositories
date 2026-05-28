namespace Skyline.DataMiner.Scripting.Helper.Converters
{
	using System;

	internal class SLBooleanConverter : ISLConverter<bool>
	{
		public bool FromRawValue(object rawValue)
		{
			var rawBool = Convert.ToInt16(rawValue);
			return Convert.ToBoolean(rawBool);
		}

		public object ToRawValue(bool value)
		{
			return Convert.ToInt16(value);
		}
	}

	internal class SLNullableBooleanConverter : ISLConverter<bool?>
	{
		public bool? FromRawValue(object rawValue)
		{
			if (rawValue == null)
			{
				return null;
			}

			var rawBool = Convert.ToInt16(rawValue);
			return Convert.ToBoolean(rawBool);
		}

		public object ToRawValue(bool? value)
		{
			if (value.HasValue)
			{
				return Convert.ToInt16(value);
			}

			return null;
		}
	}
}