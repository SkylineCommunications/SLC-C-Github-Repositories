namespace Skyline.DataMiner.Scripting.Helper
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Linq.Expressions;
	using System.Reflection;

	using Skyline.DataMiner.Scripting.Helper.Converters;

	using SLNetMessages = Skyline.DataMiner.Net.Messages;

	public class SLReadColumnObject<TDataType> : SLColumnObject<TDataType>
	{
		internal SLReadColumnObject(int columnIndex, int columnPid, SLTableWithPrimaryKey table)
			: this(columnIndex, columnPid, new HashSet<double>(0), table)
		{
		}

		internal SLReadColumnObject(int columnIndex, int columnPid, HashSet<double> exceptions, SLTableWithPrimaryKey table)
			: this(columnIndex, columnPid, SLConverters.GetConverter<TDataType>(), exceptions, table)
		{
		}

		internal SLReadColumnObject(int columnIndex, int columnPid, ISLConverter<TDataType> converter, SLTableWithPrimaryKey table)
			: this(columnIndex, columnPid, converter, new HashSet<double>(0), table)
		{
		}

		internal SLReadColumnObject(int columnIndex, int columnPid, ISLConverter<TDataType> converter, HashSet<double> exceptions, SLTableWithPrimaryKey table)
			: base(columnIndex, columnPid, converter, table)
		{
			Exceptions = exceptions ?? throw new ArgumentNullException(nameof(exceptions));
		}

		private HashSet<double> Exceptions { get; set; }

		public SLCell<TDataType> GetCell(SLProtocol protocol, string primaryKey)
		{
			Table.EnsureExists(protocol, primaryKey);

			return GetCellSafe(protocol, primaryKey);
		}

		public bool TryGetCell(SLProtocol protocol, string primaryKey, out SLCell<TDataType> cell)
		{
			if (!Table.Exists(protocol, primaryKey))
			{
				cell = default;
				return false;
			}

			cell = GetCellSafe(protocol, primaryKey);
			return true;
		}

		public void SetCell(SLProtocol protocol, string primaryKey, TDataType value)
		{
			Table.EnsureExists(protocol, primaryKey);
			SetCellSafe(protocol, primaryKey, value);
		}

		public void SetCell(SLProtocol protocol, string primaryKey, TDataType value, DateTime timeStamp)
		{
			Table.EnsureExists(protocol, primaryKey);
			SetCellSafe(protocol, primaryKey, value, timeStamp);
		}

		public bool TrySetCell(SLProtocol protocol, string primaryKey, TDataType value)
		{
			if (!Table.Exists(protocol, primaryKey))
			{
				return false;
			}

			SetCellSafe(protocol, primaryKey, value);
			return true;
		}

		public bool TrySetCell(SLProtocol protocol, string primaryKey, TDataType value, DateTime timeStamp)
		{
			if (!Table.Exists(protocol, primaryKey))
			{
				return false;
			}

			SetCellSafe(protocol, primaryKey, value, timeStamp);
			return true;
		}

		public void SetCells(SLProtocol protocol, IDictionary<string, TDataType> valueByPrimaryKey)
		{
			if (valueByPrimaryKey == null)
				throw new ArgumentNullException(nameof(valueByPrimaryKey));

			if (valueByPrimaryKey.Count == 0)
				return;

			var info = new object[]
			{
				Table.TablePid,
				ColumnPid,
				false,
			};

			var data = new object[]
			{
				valueByPrimaryKey.Keys.Cast<object>().ToArray(),
				valueByPrimaryKey.Values.Select(Converter.ToRawValue).ToArray(),
			};

			protocol.NotifyProtocol((int)SLNetMessages.NotifyType.NT_FILL_ARRAY_WITH_COLUMN, info, data);
		}

		public void SetCells(SLProtocol protocol, HashSet<string> primaryKeys, TDataType value)
		{
			if (primaryKeys == null)
				throw new ArgumentNullException(nameof(primaryKeys));

			if (primaryKeys.Count == 0)
				return;

			var keys = primaryKeys.ToArray();
			var rawValue = Converter.ToRawValue(value);

			var info = new object[]
			{
				Table.TablePid,
				ColumnPid,
				false,
			};

			var data = new object[]
			{
				keys.Cast<object>().ToArray(),
				Enumerable.Repeat(rawValue, keys.Length).ToArray(),
			};

			protocol.NotifyProtocol((int)SLNetMessages.NotifyType.NT_FILL_ARRAY_WITH_COLUMN, info, data);
		}

		public HashSet<string> GetPrimaryKeysForValue(SLProtocol protocol, TDataType value)
		{
			var columnIndexes = new uint[2]
			{
				(uint)Table.IndexColumn,
				(uint)ColumnIndex,
			};
			var columns = (object[])protocol.NotifyProtocol((int)SLNetMessages.NotifyType.NT_GET_TABLE_COLUMNS, Table.TablePid, columnIndexes);

			var primaryKeys = (object[])columns[0];
			var rawValues = (object[])columns[1];

			var comparer = EqualityComparer<TDataType>.Default;
			var result = new HashSet<string>(primaryKeys.Length);

			for (int i = 0; i < primaryKeys.Length; i++)
			{
				var convertedValue = Converter.FromRawValue(rawValues[i]);
				if (!comparer.Equals(convertedValue, value))
				{
					continue;
				}

				var primaryKey = Convert.ToString(primaryKeys[i]);
				result.Add(primaryKey);
			}

			return result;
		}

		public IDictionary<TDataType, HashSet<string>> GetPrimaryKeysByValue(SLProtocol protocol)
		{
			var columnIndexes = new uint[2]
			{
				(uint)Table.IndexColumn,
				(uint)ColumnIndex,
			};
			var columns = (object[])protocol.NotifyProtocol((int)SLNetMessages.NotifyType.NT_GET_TABLE_COLUMNS, Table.TablePid, columnIndexes);

			var primaryKeys = (object[])columns[0];
			var rawValues = (object[])columns[1];

			var result = new Dictionary<TDataType, HashSet<string>>();

			for (int i = 0; i < primaryKeys.Length; i++)
			{
				var value = Converter.FromRawValue(rawValues[i]);

				if (!result.TryGetValue(value, out HashSet<string> primaryKeySet))
				{
					primaryKeySet = new HashSet<string>();
					result[value] = primaryKeySet;
				}

				var primaryKey = Table.IndexConverter.FromRawValue(primaryKeys[i]);
				primaryKeySet.Add(primaryKey);
			}

			return result;
		}

		public IDictionary<string, TDataType> GetValueByPrimaryKey(SLProtocol protocol)
		{
			var columnIndexes = new uint[2]
			{
				(uint)Table.IndexColumn,
				(uint)ColumnIndex,
			};
			var columns = (object[])protocol.NotifyProtocol((int)SLNetMessages.NotifyType.NT_GET_TABLE_COLUMNS, Table.TablePid, columnIndexes);

			var primaryKeys = (object[])columns[0];
			var rawValues = (object[])columns[1];

			var rowCount = primaryKeys.Length;
			var result = new Dictionary<string, TDataType>(rowCount);

			for (int i = 0; i < rowCount; i++)
			{
				var primaryKey = Convert.ToString(primaryKeys[i]);
				var value = Converter.FromRawValue(rawValues[i]);

				result.Add(primaryKey, value);
			}

			return result;
		}

		public IDictionary<string, TDataType> GetValuesByPrimaryKeys(SLProtocol protocol, HashSet<string> primaryKeys)
		{
			var columnIndexes = new uint[2]
			{
				(uint)Table.IndexColumn,
				(uint)ColumnIndex,
			};
			var columns = (object[])protocol.NotifyProtocol((int)SLNetMessages.NotifyType.NT_GET_TABLE_COLUMNS, Table.TablePid, columnIndexes);

			var keys = (object[])columns[0];
			var rawValues = (object[])columns[1];

			var rowCount = primaryKeys.Count;
			var result = new Dictionary<string, TDataType>(rowCount);

			for (int i = 0; i < rowCount; i++)
			{
				var primaryKey = Table.IndexConverter.FromRawValue(keys[i]);
				if (!primaryKeys.Contains(primaryKey))
				{
					continue;
				}

				var value = Converter.FromRawValue(rawValues[i]);
				result.Add(primaryKey, value);
			}

			return result;
		}

		public IList<TDataType> GetCells(SLProtocol protocol)
		{
			var columnIndexes = new uint[1]
			{
				(uint)ColumnIndex,
			};
			var columns = (object[])protocol.NotifyProtocol((int)SLNetMessages.NotifyType.NT_GET_TABLE_COLUMNS, Table.TablePid, columnIndexes);

			var rawValues = (object[])columns[0];

			var list = new TDataType[rawValues.Length];

			for (int i = 0; i < rawValues.Length; i++)
			{
				list[i] = Converter.FromRawValue(rawValues[i]);
			}

			return list;
		}

		public ColumnPropertyMap<TModel, TDataType> Map<TModel>(Expression<Func<TModel, TDataType>> propertyExpr)
			where TModel : new()
		{
			var propertyAccess = propertyExpr.Body as MemberExpression;
			if (propertyExpr.Body is UnaryExpression unary && unary.NodeType == ExpressionType.Convert)
			{
				// This happens when boxing happened on a value type.
				// E.g. DateTime is boxed to object, and then the expression is cast back to DateTime.
				propertyAccess = unary.Operand as MemberExpression;
			}

			if (propertyAccess is null)
				throw new ArgumentException("Expression must be a property access", nameof(propertyExpr));

			if (!(propertyAccess.Member is PropertyInfo propertyInfo))
				throw new ArgumentException("Expression must be a property", nameof(propertyExpr));

			Action<TModel, TDataType> assign = (model, value) => propertyInfo.SetValue(model, value);

			return new ColumnPropertyMap<TModel, TDataType>(this, assign);
		}

		public ColumnPropertyWriteMap<TModel, TDataType> MapWrite<TModel>(Expression<Func<TModel, TDataType>> propertyExpr)
		{
			var propertyAccess = propertyExpr.Body as MemberExpression;
			if (propertyExpr.Body is UnaryExpression unary && unary.NodeType == ExpressionType.Convert)
			{
				// This happens when boxing happened on a value type.
				// E.g. DateTime is boxed to object, and then the expression is cast back to DateTime.
				propertyAccess = unary.Operand as MemberExpression;
			}

			if (propertyAccess is null)
				throw new ArgumentException("Expression must be a property access", nameof(propertyExpr));

			if (!(propertyAccess.Member is PropertyInfo propertyInfo))
				throw new ArgumentException("Expression must be a property", nameof(propertyExpr));

			// Getter for the property on the model
			Func<TModel, TDataType> getter =
				model => (TDataType)propertyInfo.GetValue(model);

			// `this` must be an SLWriteColumnObject<TDataType>
			return new ColumnPropertyWriteMap<TModel, TDataType>((SLReadColumnObject<TDataType>)this, getter);
		}

		private void SetCellSafe(SLProtocol protocol, string primaryKey, TDataType value)
		{
			var rawValue = Converter.ToRawValue(value);
			protocol.SetParameterIndexByKey(Table.TablePid, primaryKey, OneBasedColumnIndex, rawValue);
		}

		private void SetCellSafe(SLProtocol protocol, string primaryKey, TDataType value, DateTime timeStamp)
		{
			var rawValue = Converter.ToRawValue(value);
			protocol.SetParameterIndexByKey(Table.TablePid, primaryKey, OneBasedColumnIndex, rawValue, timeStamp);
		}

		private SLCell<TDataType> GetCellSafe(SLProtocol protocol, string primaryKey)
		{
			var rawValue = protocol.GetParameterIndexByKey(
				Table.TablePid,
				primaryKey,
				OneBasedColumnIndex);

			// Empty cell
			if (rawValue == null)
				return SLCell<TDataType>.Empty();

			// Exception value
			if (rawValue is double v && Exceptions.Contains(v))
				return SLCell<TDataType>.Exception(rawValue);

			// Normal value
			var typedValue = Converter.FromRawValue(rawValue);
			return SLCell<TDataType>.Valid(typedValue);
		}
	}
}