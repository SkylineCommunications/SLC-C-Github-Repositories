namespace Skyline.Protocol.Tables
{
	using System;
	using System.Collections.Generic;
	using System.Linq;

	using Skyline.DataMiner.Scripting.Helper;

	internal class TopicsConverter : ISLConverter<List<string>>
	{
		public List<string> FromRawValue(object rawValue)
		{
			var value = Convert.ToString(rawValue);
			if (string.IsNullOrWhiteSpace(value))
			{
				return new List<string>();
			}

			return value.Split(',').ToList();
		}

		public object ToRawValue(List<string> value)
		{
			if (value == null || value.Count == 0)
			{
				return string.Empty;
			}

			return string.Join(",", value);
		}
	}
}
