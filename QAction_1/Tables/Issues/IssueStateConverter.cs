namespace Skyline.Protocol.Tables
{
	using System;

	using Skyline.DataMiner.Scripting.Helper;

	public enum IssueState
	{
		Closed,
		Open,
	}

	internal class IssueStateConverter : ISLConverter<IssueState>
	{
		public IssueState FromRawValue(object rawValue)
		{
			var value = Convert.ToString(rawValue);
			if (string.IsNullOrEmpty(value))
			{
				return IssueState.Open;
			}

			return (IssueState)Enum.Parse(typeof(IssueState), value, true);
		}

		public object ToRawValue(IssueState value)
		{
			return Enum.GetName(typeof(IssueState), value).ToLower();
		}
	}
}
