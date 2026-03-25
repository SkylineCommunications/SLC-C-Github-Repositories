namespace Skyline.DataMiner.Scripting.Helper
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Linq.Expressions;
	using System.Threading.Tasks;

	using Skyline.DataMiner.Scripting;
	using Skyline.DataMiner.Scripting.Helper.Converters;
	using Skyline.DataMiner.Scripting.Helper.Events;
	using Skyline.DataMiner.Scripting.Helper.Exceptions;

	using static Skyline.DataMiner.Scripting.NotifyProtocol;

	using SLNetMessages = Skyline.DataMiner.Net.Messages;

	public class SLTable
	{
		protected SLTable(int tablePid)
		{
			TablePid = tablePid;
		}

		public int TablePid { get; private set; }

		public bool Exists(SLProtocol protocol, string primaryKey)
		{
			if (primaryKey == null)
				throw new ArgumentNullException(nameof(primaryKey));

			return protocol.Exists(TablePid, primaryKey);
		}

		internal void EnsureExists(SLProtocol protocol, string primaryKey)
		{
			if (!Exists(protocol, primaryKey))
				throw new PrimaryKeyNotFoundException(TablePid, primaryKey);
		}
	}

	public class SLTableWithPrimaryKey : SLTable
	{
		protected SLTableWithPrimaryKey(int tablePid, int indexColumn, int indexColumnPid)
			: base(tablePid)
		{
			IndexColumn = indexColumn;
			IndexColumnPid = indexColumnPid;
			IndexConverter = new SLStringConverter();
		}

		public int IndexColumn { get; }

		public int IndexColumnPid { get; }

		internal ISLConverter<string> IndexConverter { get; }

		public IList<string> GetPrimaryKeys(SLProtocol protocol)
		{
			return protocol.GetKeys(TablePid);
		}
	}

	public abstract class SLTable<TRow> : SLTableWithPrimaryKey, IDisposable
		where TRow : QActionTableRow, new()
	{
		protected SLTable(int tablePid, int indexColumn, int indexColumnPid)
			: base(tablePid, indexColumn, indexColumnPid)
		{
		}

		public event EventHandler<DeleteRowEventArgs> RowDeleted;

		public void AddRow(SLProtocol protocol, string primaryKey)
		{
			if (Exists(protocol, primaryKey))
				throw new PrimaryKeyAlreadyExistsException(TablePid, primaryKey);

			protocol.AddRow(TablePid, primaryKey);
		}

		public void AddRow(SLProtocol protocol, TRow row)
		{
			if (row == null)
				throw new ArgumentNullException(nameof(row));

			if (Exists(protocol, row.Key))
				throw new PrimaryKeyAlreadyExistsException(TablePid, row.Key);

			protocol.AddRow(TablePid, row.ToObjectArray());
		}

		public void AddRow(SLProtocol protocol, TRow row, DateTime timeStamp)
		{
			if (row == null)
				throw new ArgumentNullException(nameof(row));

			if (Exists(protocol, row.Key))
				throw new PrimaryKeyAlreadyExistsException(TablePid, row.Key);

			protocol.AddRow(TablePid, new object[] { row.ToObjectArray(), timeStamp });
		}

		public bool TryAddRow(SLProtocol protocol, string primaryKey)
		{
			if (Exists(protocol, primaryKey))
				return false;

			protocol.AddRow(TablePid, primaryKey);

			return true;
		}

		public bool TryAddRow(SLProtocol protocol, TRow row)
		{
			if (row == null)
				throw new ArgumentNullException(nameof(row));

			if (Exists(protocol, row.Key))
				return false;

			protocol.AddRow(TablePid, row.ToObjectArray());

			return true;
		}

		public bool TryAddRow(SLProtocol protocol, TRow row, DateTime timeStamp)
		{
			if (row == null)
				throw new ArgumentNullException(nameof(row));

			if (Exists(protocol, row.Key))
				return false;

			protocol.AddRow(TablePid, new object[] { row.ToObjectArray(), timeStamp });

			return true;
		}

		public string AddRowReturnKey(SLProtocol protocol)
		{
			return protocol.AddRowReturnKey(TablePid);
		}

		public TRow GetRow(SLProtocol protocol, string primaryKey)
		{
			EnsureExists(protocol, primaryKey);

			var raw = (object[])protocol.GetRow(TablePid, primaryKey);
			return (TRow)Activator.CreateInstance(typeof(TRow), new object[] { raw });
		}

		public bool TryGetRow(SLProtocol protocol, string primaryKey, out TRow row)
		{
			if (!Exists(protocol, primaryKey))
			{
				row = default;
				return false;
			}

			var raw = (object[])protocol.GetRow(TablePid, primaryKey);
			row = (TRow)Activator.CreateInstance(typeof(TRow), new object[] { raw });
			return true;
		}

		public void SetRow(SLProtocol protocol, TRow row)
		{
			if (row == null)
				throw new ArgumentNullException(nameof(row));

			EnsureExists(protocol, row.Key);
			protocol.SetRow(TablePid, row.Key, row.ToObjectArray());
		}

		public void SetRow(SLProtocol protocol, TRow row, bool ov)
		{
			if (row == null)
				throw new ArgumentNullException(nameof(row));

			EnsureExists(protocol, row.Key);
			protocol.SetRow(TablePid, row.Key, row.ToObjectArray(), ov);
		}

		public void SetRow(SLProtocol protocol, TRow row, DateTime timeStamp)
		{
			if (row == null)
				throw new ArgumentNullException(nameof(row));

			EnsureExists(protocol, row.Key);
			protocol.SetRow(TablePid, row.Key, row.ToObjectArray(), timeStamp);
		}

		public void SetRow(SLProtocol protocol, TRow row, DateTime timeStamp, bool ov)
		{
			if (row == null)
				throw new ArgumentNullException(nameof(row));

			EnsureExists(protocol, row.Key);
			protocol.SetRow(TablePid, row.Key, row.ToObjectArray(), timeStamp, ov);
		}

		public bool TrySetRow(SLProtocol protocol, TRow row)
		{
			if (row == null)
				throw new ArgumentNullException(nameof(row));

			if (!Exists(protocol, row.Key))
			{
				return false;
			}

			protocol.SetRow(TablePid, row.Key, row.ToObjectArray());

			return true;
		}

		public bool TrySetRow(SLProtocol protocol, TRow row, bool ov)
		{
			if (row == null)
				throw new ArgumentNullException(nameof(row));

			if (!Exists(protocol, row.Key))
			{
				return false;
			}

			protocol.SetRow(TablePid, row.Key, row.ToObjectArray(), ov);

			return true;
		}

		public bool TrySetRow(SLProtocol protocol, TRow row, DateTime timeStamp)
		{
			if (row == null)
				throw new ArgumentNullException(nameof(row));

			if (!Exists(protocol, row.Key))
			{
				return false;
			}

			protocol.SetRow(TablePid, row.Key, row.ToObjectArray(), timeStamp);

			return true;
		}

		public bool TrySetRow(SLProtocol protocol, TRow row, DateTime timeStamp, bool ov)
		{
			if (row == null)
				throw new ArgumentNullException(nameof(row));

			if (!Exists(protocol, row.Key))
			{
				return false;
			}

			protocol.SetRow(TablePid, row.Key, row.ToObjectArray(), timeStamp, ov);

			return true;
		}

		public int DeleteRow(SLProtocol protocol, string primaryKey)
		{
			EnsureExists(protocol, primaryKey);

			var result = protocol.DeleteRow(TablePid, primaryKey);
			RowDeleted?.Invoke(this, new DeleteRowEventArgs(protocol, primaryKey));
			return result;
		}

		public bool TryDeleteRow(SLProtocol protocol, string primaryKey, out int? rowCount)
		{
			if (!Exists(protocol, primaryKey))
			{
				rowCount = default;
				return false;
			}

			rowCount = protocol.DeleteRow(TablePid, primaryKey);
			RowDeleted?.Invoke(this, new DeleteRowEventArgs(protocol, primaryKey));
			return true;
		}

		public int? DeleteRows(SLProtocol protocol, HashSet<string> primaryKeys)
		{
			if (primaryKeys == null)
				throw new ArgumentNullException(nameof(primaryKeys));

			var arr = primaryKeys.ToArray();
			if (arr.Length == 0)
				return null;

			var result = protocol.DeleteRow(TablePid, arr);
			RowDeleted?.Invoke(this, new DeleteRowEventArgs(protocol, primaryKeys));
			return result;
		}

		public void ClearAllKeys(SLProtocol protocol)
		{
			var primaryKeys = GetPrimaryKeys(protocol);
			protocol.ClearAllKeys(TablePid);
			RowDeleted?.Invoke(this, new DeleteRowEventArgs(protocol, primaryKeys.ToHashSet()));
		}

		////public void FillTable(SLProtocol protocol, IEnumerable<TRow> rows)
		////{
		////	FillTableInternal(protocol, rows, null);
		////}

		////public void FillTable(SLProtocol protocol, IEnumerable<TRow> rows, DateTime timeStamp)
		////{
		////	FillTableInternal(protocol, rows, timeStamp);
		////}

		public void FillTableNoDelete(SLProtocol protocol, IEnumerable<TRow> rows)
		{
			FillTableNoDeleteInternal(protocol, rows, null);
		}

		public void FillTableNoDelete(SLProtocol protocol, IEnumerable<TRow> rows, DateTime timeStamp)
		{
			FillTableNoDeleteInternal(protocol, rows, timeStamp);
		}

		private void FillTableInternal(SLProtocol protocol, IEnumerable<TRow> rows, DateTime? timeStamp)
		{
			if (rows == null)
				throw new ArgumentNullException(nameof(rows));

			if (!rows.Any())
				return;

			var hasEmptyRows = rows.Any(row => row is null);
			if (hasEmptyRows)
				throw new ArgumentException("Collection cannot contain items that are 'null'.", nameof(rows));

			var primaryKeys = rows.Select(r => r.Key).ToArray();
			EnsureNoDuplicateKeys(primaryKeys);

			List<object[]> data = rows.Select(r => r.ToObjectArray()).ToList();

			var batches = CreateBatches(data);
			if (batches.Count() > 1)
			{
				var pks = protocol.GetKeys(TablePid);
				foreach (var batch in batches)
				{
					if (!timeStamp.HasValue)
					{
						protocol.FillArray(TablePid, batch, SaveOption.Partial);
					}
					else
					{
						protocol.FillArray(TablePid, batch, SaveOption.Partial, timeStamp.Value);
					}
				}

				var keysToDelete = pks.Except(primaryKeys).ToArray();
				protocol.DeleteRow(TablePid, keysToDelete);
			}
			else
			{
				if (!timeStamp.HasValue)
				{
					protocol.FillArray(TablePid, data, SaveOption.Full);
				}
				else
				{
					protocol.FillArray(TablePid, data, SaveOption.Full, timeStamp.Value);
				}
			}
		}

		private void FillTableNoDeleteInternal(SLProtocol protocol, IEnumerable<TRow> rows, DateTime? timeStamp)
		{
			if (rows == null)
				throw new ArgumentNullException(nameof(rows));

			if (!rows.Any())
				return;

			var hasEmptyRows = rows.Any(row => row is null);
			if (hasEmptyRows)
				throw new ArgumentException("Collection cannot contain items that are 'null'.", nameof(rows));

			var primaryKeys = rows.Select(r => r.Key).ToArray();
			EnsureNoDuplicateKeys(primaryKeys);

			List<object[]> data = rows.Select(r => r.ToObjectArray()).ToList();

			var batches = CreateBatches(data);
			foreach (var batch in batches)
			{
				if (!timeStamp.HasValue)
				{
					protocol.FillArray(TablePid, batch, SaveOption.Partial);
				}
				else
				{
					protocol.FillArray(TablePid, batch, SaveOption.Partial, timeStamp.Value);
				}
			}
		}

		public int RowCount(SLProtocol protocol)
		{
			return protocol.RowCount(TablePid);
		}

		public bool IsEmpty(SLProtocol protocol)
		{
			return RowCount(protocol) == 0;
		}

		public IList<TModel> GetData<TModel>(SLProtocol protocol, params ColumnMapBase<TModel>[] maps)
			where TModel : new()
		{
			if (maps == null)
				throw new ArgumentNullException(nameof(maps));

			int columnCount = maps.Length;
			if (columnCount == 0)
				throw new ArgumentException("No maps provided", nameof(maps));

			var hasMismatch = maps.Any(map => map.Column.Table.TablePid != TablePid);
			if (hasMismatch)
				throw new ArgumentException("One or more column maps belong to a different table than the target table.", nameof(maps));

			var columnIndexes = Array.ConvertAll(maps, m => (uint)m.Column.ColumnIndex);

			var columns = (object[])protocol.NotifyProtocol(
				(int)SLNetMessages.NotifyType.NT_GET_TABLE_COLUMNS,
				TablePid,
				columnIndexes);

			return ConvertModelData(maps, columnCount, columns);
		}

		public void SetData<TModel>(
			SLProtocol protocol,
			IEnumerable<TModel> models,
			Func<TModel, string> keySelector,
			params ColumnWriteMapBase<TModel>[] maps)
		{
			if (models == null)
				throw new ArgumentNullException(nameof(models));

			if (keySelector == null)
				throw new ArgumentNullException(nameof(keySelector));

			if (maps == null || maps.Length == 0)
				throw new ArgumentException("No write maps provided", nameof(maps));

			// Validate table
			if (maps.Any(m => m.Column.Table.TablePid != TablePid))
				throw new ArgumentException("Column maps belong to a different table", nameof(maps));

			var list = models.ToList();
			if (list.Count == 0)
				return;

			object[] aoKeys = list
				.Select(m => (object)keySelector(m).ToString())
				.ToArray();

			object[] pidInfo = new object[maps.Length + 1];
			pidInfo[0] = TablePid;

			for (int i = 0; i < maps.Length; i++)
				pidInfo[i + 1] = maps[i].Column.ColumnPid;

			object[] valueInfo = new object[maps.Length + 1];
			valueInfo[0] = aoKeys;

			for (int i = 0; i < maps.Length; i++)
			{
				var map = maps[i];
				valueInfo[i + 1] = list
					.Select(model => map.GetRaw(model))
					.ToArray();
			}

			protocol.NotifyProtocol((int)SLNetMessages.NotifyType.NT_FILL_ARRAY_WITH_COLUMN, pidInfo, valueInfo);
		}

		private static void EnsureNoDuplicateKeys(string[] keys)
		{
			var dup = keys.GroupBy(x => x).Where(g => g.Count() > 1).Select(g => g.Key).ToArray();
			if (dup.Length > 0)
			{
				throw new ArgumentException("Duplicate primary keys:\n" + string.Join("\n", dup));
			}
		}

		private static IList<TModel> ConvertModelData<TModel>(ColumnMapBase<TModel>[] maps, int columnCount, object[] columns)
			where TModel : new()
		{
			var dataColumns = new object[columnCount][];
			var applyDelegates = new Action<TModel, object>[columnCount];

			for (int iColumn = 0; iColumn < columnCount; iColumn++)
			{
				var map = maps[iColumn];
				dataColumns[iColumn] = (object[])columns[iColumn];
				applyDelegates[iColumn] = (model, val) => map.Apply(model, val);
			}

			int rowCount = dataColumns[0].Length;
			var results = new TModel[rowCount];
			var create = ModelFactory<TModel>.Create;

			Parallel.For(0, rowCount, new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount }, row =>
			{
				var model = create();

				for (int m = 0; m < columnCount; m++)
				{
					var apply = applyDelegates[m];
					var col = dataColumns[m];
					var val = col[row];

					apply(model, val);
				}

				results[row] = model;
			});

			return results;
		}

		private void FillArray(SLProtocol protocol, IEnumerable<TRow> rows, NotifyProtocol.SaveOption saveOption)
		{
			FillArray(protocol, rows, saveOption, null);
		}

		private void FillArray(SLProtocol protocol, IEnumerable<TRow> rows, NotifyProtocol.SaveOption saveOption, DateTime? timeStamp)
		{
			if (rows == null)
				throw new ArgumentNullException(nameof(rows));

			var hasEmptyRows = rows.Any(row => row is null);
			if (hasEmptyRows)
				throw new ArgumentException("Collection cannot contain items that are 'null'.", nameof(rows));

			if (saveOption == SaveOption.Full)
			{
				throw new ArgumentException("This connector only support Partial save option. Use FillTableInternal for full updates.", nameof(saveOption));
			}

			if (saveOption == NotifyProtocol.SaveOption.Partial && !rows.Any())
			{
				return;
			}

			var primaryKeys = rows.Select(r => r.Key).ToArray();
			EnsureNoDuplicateKeys(primaryKeys);

			List<object[]> data = rows.Select(r => r.ToObjectArray()).ToList();

			const int maxCellCount = 50_000;

			var cellCount = data.Sum(r => r.Length);

			if (cellCount <= maxCellCount)
			{
				if (!timeStamp.HasValue)
				{
					protocol.FillArray(TablePid, data, saveOption);
					return;
				}

				protocol.FillArray(TablePid, data, saveOption, timeStamp);

				return;
			}
		}

		private static IEnumerable<List<object[]>> CreateBatches(List<object[]> data)
		{
			const int maxCells = 50_000;

			var cellCount = data.Sum(r => r.Length);
			if (cellCount <= maxCells)
			{
				yield return data;
			}

			var currentBatch = new List<object[]>();
			int currentCellCount = 0;

			foreach (var row in data)
			{
				int rowCellCount = row.Length;

				// If adding this row would exceed limit -> start a new batch
				if (currentCellCount + rowCellCount > maxCells && currentBatch.Count > 0)
				{
					yield return currentBatch;
					currentBatch = new List<object[]>();
					currentCellCount = 0;
				}

				currentBatch.Add(row);
				currentCellCount += rowCellCount;
			}

			// Return final batch
			if (currentBatch.Count > 0)
				yield return currentBatch;
		}

		public void Dispose()
		{
			try
			{
				DisposeEvents();
			}
			catch (Exception)
			{
				// Dispose should not throw exceptions
			}
		}

		protected abstract void DisposeEvents();

		private static class ModelFactory<T>
			where T : new()
		{
			public static readonly Func<T> Create = CreateFactory();

			private static Func<T> CreateFactory()
			{
				var ctor = typeof(T).GetConstructor(Type.EmptyTypes);
				var exp = Expression.New(ctor);

				return Expression.Lambda<Func<T>>(exp).Compile();
			}
		}
	}
}