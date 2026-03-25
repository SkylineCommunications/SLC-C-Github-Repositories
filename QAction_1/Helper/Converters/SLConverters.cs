namespace Skyline.DataMiner.Scripting.Helper.Converters
{
	using System;
	using System.Collections.Generic;

	internal static class SLConverters
	{
		private static readonly IReadOnlyDictionary<Type, object> ConverterByType = new Dictionary<Type, object>
		{
			{ typeof(string), new SLStringConverter() },
			{ typeof(DateTime?), new SLDateTimeConverter() },
			{ typeof(short?), new SLInt16Converter() },
			{ typeof(int?), new SLInt32Converter() },
			{ typeof(long?), new SLInt64Converter() },
			{ typeof(ushort?), new SLUInt16Converter() },
			{ typeof(uint?), new SLUInt32Converter() },
			{ typeof(ulong?), new SLUInt64Converter() },
			{ typeof(TimeSpan?), new SLTimeSpanConverter() },
			{ typeof(double?), new SLDoubleConverter() },
			{ typeof(bool), new SLBooleanConverter() },
			{ typeof(bool?), new SLNullableBooleanConverter() },
		};

		internal static ISLConverter<TValue> GetConverter<TValue>()
		{
			object converter;
			if (ConverterByType.TryGetValue(typeof(TValue), out converter))
			{
				return (ISLConverter<TValue>)converter;
			}

			throw new Exception("No converter registered for type " + typeof(TValue).FullName);
		}
	}
}