namespace Skyline.DataMiner.Scripting.Helper
{
	public interface ISLConverter<TValue>
	{
		TValue FromRawValue(object rawValue);

		object ToRawValue(TValue value);
	}

	public interface ISLRowConverter<TValue, THelper>
		where THelper : QActionTableRow, new()
	{
		TValue FromRawValue(THelper rawRow);

		THelper ToRawValue(TValue row);
	}
}