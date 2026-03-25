namespace Skyline.DataMiner.Scripting.Helper
{
	using System;

	public abstract class SLColumnObject
	{
		protected SLColumnObject(int index, int pid, SLTableWithPrimaryKey table)
		{
			ColumnIndex = index;
			ColumnPid = pid;
			Table = table ?? throw new ArgumentNullException(nameof(table));
		}

		public int ColumnIndex { get; }

		public int ColumnPid { get; }

		internal int OneBasedColumnIndex => ColumnIndex + 1;

		internal SLTableWithPrimaryKey Table { get; }
	}

	public abstract class SLColumnObject<TDataType> : SLColumnObject
	{
		protected SLColumnObject(int index, int pid, ISLConverter<TDataType> converter, SLTableWithPrimaryKey table)
			: base(index, pid, table)
		{
			Converter = converter ?? throw new ArgumentNullException(nameof(converter));
		}

		internal ISLConverter<TDataType> Converter { get; }
	}
}