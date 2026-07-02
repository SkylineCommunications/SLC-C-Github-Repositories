namespace Skyline.DataMiner.Scripting.Helper
{
	using System;
	using System.Linq.Expressions;
	using System.Reflection;

	using Skyline.DataMiner.Net.SLDataGateway.Types;

	public static class ColumnWriteExtensions
	{
		public static ColumnPropertyWriteMap<TModel, TValue> Map<TModel, TValue>(
			this SLReadColumnObject<TValue> col,
			Func<TModel, TValue> getter)
			=> new ColumnPropertyWriteMap<TModel, TValue>(col, getter);
	}

	public abstract class ColumnMapBase<TModel>
	{
		protected ColumnMapBase(SLColumnObject column)
		{
			Column = column ?? throw new ArgumentNullException(nameof(column));
		}

		internal SLColumnObject Column { get; }

		internal abstract void Apply(TModel model, object raw);
	}

	public class ColumnPropertyMap<TModel, TValue> : ColumnMapBase<TModel>
	{
		private readonly SLReadColumnObject<TValue> _typedColumn;

		private readonly Action<TModel, TValue> _assign;

		internal ColumnPropertyMap(SLReadColumnObject<TValue> column, Action<TModel, TValue> assign)
			: base(column)
		{
			_typedColumn = column;
			_assign = assign;
		}

		internal override void Apply(TModel model, object raw)
		{
			_assign(model, _typedColumn.Converter.FromRawValue(raw));
		}
	}

	public abstract class ColumnWriteMapBase<TModel>
	{
		protected ColumnWriteMapBase(SLColumnObject column)
		{
			Column = column ?? throw new ArgumentNullException(nameof(column));
		}

		internal SLColumnObject Column { get; }

		internal abstract object GetRaw(TModel model);
	}

	public sealed class ColumnPropertyWriteMap<TModel, TValue> : ColumnWriteMapBase<TModel>
	{
		private readonly SLReadColumnObject<TValue> _typedColumn;
		private readonly Func<TModel, TValue> _getter;

		public ColumnPropertyWriteMap(
			SLReadColumnObject<TValue> column,
			Func<TModel, TValue> getter)
			: base(column)
		{
			_typedColumn = column;
			_getter = getter;
		}

		internal override object GetRaw(TModel model)
		{
			return _typedColumn.Converter.ToRawValue(_getter(model));
		}
	}
}