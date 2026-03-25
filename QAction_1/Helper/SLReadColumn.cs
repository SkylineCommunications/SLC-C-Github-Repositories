namespace Skyline.DataMiner.Scripting.Helper
{
	using Skyline.DataMiner.Scripting.Helper.Converters;

	public class SLReadColumn<TDataType>
	{
		public SLReadColumn(int columnIndex, int columnPid, SLTableWithPrimaryKey table)
			: this(columnIndex, columnPid, SLConverters.GetConverter<TDataType>(), table)
		{
		}

		public SLReadColumn(int columnIndex, int columnPid, ISLConverter<TDataType> converter, SLTableWithPrimaryKey table)
		{
			Read = new SLReadColumnObject<TDataType>(columnIndex, columnPid, converter, table);
		}

		public SLReadColumnObject<TDataType> Read { get; private set; }

		public TDataType ReadFromRow(object[] row) => Read.Converter.FromRawValue(row[Read.ColumnIndex]);
	}
}