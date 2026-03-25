namespace Skyline.Protocol.Tables
{
	using System;
	using System.ComponentModel;

	using Skyline.DataMiner.Scripting.Helper;
	using Skyline.Protocol.Extensions;

	using Extensions = Skyline.Protocol.Extensions.Extensions;

	public enum PrivacySetting
	{
		[Description("closed")]
		Closed,

		[Description("secret")]
		Secret,
	}

	public enum NotificationSetting
	{
		[Description("notifications_enabled")]
		Enabled,

		[Description("notifications_disabled")]
		Disabled,
	}

	public class PrivacySettingConverter : ISLConverter<PrivacySetting?>
	{
		public static readonly PrivacySettingConverter Instance = new PrivacySettingConverter();

		public object ToRawValue(PrivacySetting? value)
		{
			return value?.FriendlyDescription();
		}

		public PrivacySetting? FromRawValue(object raw)
		{
			if (raw == null)
			{
				return null;
			}

			var stringValue = Convert.ToString(raw);
			if (string.IsNullOrWhiteSpace(stringValue))
			{
				return null;
			}

			return Extensions.ParseNullableEnumDescription<PrivacySetting>(stringValue);
		}
	}

	public class NotificationSettingConverter : ISLConverter<NotificationSetting?>
	{
		public static readonly NotificationSettingConverter Instance = new NotificationSettingConverter();

		public object ToRawValue(NotificationSetting? value)
		{
			return value?.FriendlyDescription();
		}

		public NotificationSetting? FromRawValue(object raw)
		{
			if (raw == null)
			{
				return null;
			}

			var stringValue = Convert.ToString(raw);
			if (string.IsNullOrWhiteSpace(stringValue))
			{
				return null;
			}

			return Extensions.ParseNullableEnumDescription<NotificationSetting>(stringValue);
		}
	}
}
