namespace Skyline.Protocol.Tables
{
	using System;

	using Skyline.DataMiner.Scripting.Helper;

	internal class RepositoryTypeConverter : ISLConverter<RepositoryType?>
	{
		public RepositoryType? FromRawValue(object rawValue)
		{
			if (rawValue == null)
			{
				return null;
			}

			return (RepositoryType)Convert.ToInt32(rawValue);
		}

		public object ToRawValue(RepositoryType? value)
		{
			if (!value.HasValue)
			{
				return null;
			}

			return Convert.ToDouble((int)value);
		}
	}
}
