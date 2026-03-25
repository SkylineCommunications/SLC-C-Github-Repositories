namespace Skyline.DataMiner.Scripting.Helper.Converters
{
	public class SLStringConverter : ISLConverter<string>
	{
		public string FromRawValue(object rawValue)
		{
			return rawValue?.ToString();
		}

		public object ToRawValue(string value)
		{
			return value ?? (object)null;
		}
	}
}