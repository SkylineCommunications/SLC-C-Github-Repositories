namespace Skyline.DataMiner.Scripting.Helper
{
	using Skyline.DataMiner.Scripting.Helper.Converters;

	public class SLWriteColumnObject<TDataType> : SLColumnObject<TDataType>
	{
		internal SLWriteColumnObject(int columnPid, SLTableWithPrimaryKey table)
			: this(columnPid, SLConverters.GetConverter<TDataType>(), table)
		{
		}

		internal SLWriteColumnObject(int columnPid, ISLConverter<TDataType> converter, SLTableWithPrimaryKey table)
			: base(default, columnPid, converter, table)
		{
		}
	}
}