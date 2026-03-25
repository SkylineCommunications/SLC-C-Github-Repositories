namespace Skyline.DataMiner.Scripting.Helper
{
	public class SLCell<TDataType>
	{
		public bool HasValue { get; private set; }

		public bool HasExceptionValue { get; private set; }

		public bool IsNotInitialized => !HasValue && !HasExceptionValue;

		public TDataType Value { get; private set; }

		public object ExceptionRawValue { get; private set; }

		internal static SLCell<TDataType> Empty()
		{
			return new SLCell<TDataType>()
			{
				HasValue = false,
				HasExceptionValue = false,
				Value = default,
				ExceptionRawValue = default,
			};
		}

		internal static SLCell<TDataType> Valid(TDataType value)
		{
			return new SLCell<TDataType>()
			{
				HasValue = true,
				HasExceptionValue = false,
				Value = value,
				ExceptionRawValue = default,
			};
		}

		internal static SLCell<TDataType> Exception(object rawExceptionValue)
		{
			return new SLCell<TDataType>()
			{
				HasValue = false,
				HasExceptionValue = true,
				Value = default,
				ExceptionRawValue = rawExceptionValue,
			};
		}
	}
}