namespace Skyline.DataMiner.Scripting.Helper
{
	public class SLWriteColumn<TDataType>
	{
		public SLWriteColumn(int columnPid, SLTableWithPrimaryKey table)
		{
			Write = new SLWriteColumnObject<TDataType>(columnPid, table);
		}

		public SLWriteColumnObject<TDataType> Write { get; private set; }
	}
}