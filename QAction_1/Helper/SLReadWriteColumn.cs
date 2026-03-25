namespace Skyline.DataMiner.Scripting.Helper
{
	public class SLReadWriteColumn<TReadDataType, TWriteDataType>
	{
		public SLReadWriteColumn(int columnIndex, int columnPid, int writeColumnPid, SLTableWithPrimaryKey table)
		{
			Read = new SLReadColumnObject<TReadDataType>(columnIndex, columnPid, table);
			Write = new SLWriteColumnObject<TWriteDataType>(writeColumnPid, table);
		}

		public SLReadColumnObject<TReadDataType> Read { get; private set; }

		public SLWriteColumnObject<TWriteDataType> Write { get; private set; }
	}
}